import base64
import numpy as np
import cv2
from pathlib import Path
from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from ultralytics import YOLO

# Diretório base = pasta onde este script está (garante caminhos corretos)
DIRETORIO_BASE = Path(__file__).resolve().parent

app = FastAPI(title="Serviço de Inferência Detecta IA", version="1.0.0")

# Configura CORS para permitir chamadas do frontend Angular local
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Carrega o modelo YOLOv8n (versão nano, mais leve para tempo real)
# ultralytics baixa automaticamente na primeira execução
modelo_yolo = YOLO(str(DIRETORIO_BASE / "yolov8n.pt"))


# Carrega o modelo customizado banana_prata.pt
caminho_banana = DIRETORIO_BASE / "banana_prata.pt"
try:
    modelo_banana = YOLO(str(caminho_banana))
    print(f"Modelo banana_prata.pt carregado com classes: {modelo_banana.names}")
except Exception as e:
    modelo_banana = None
    print(f"Falha ao carregar modelo banana_prata.pt ({caminho_banana}): {e}")

# Carrega o modelo customizado banana_nanica.pt
caminho_nanica = DIRETORIO_BASE / "banana_nanica.pt"
try:
    modelo_nanica = YOLO(str(caminho_nanica))
    print(f"Modelo banana_nanica.pt carregado com classes: {modelo_nanica.names}")
except Exception as e:
    modelo_nanica = None
    print(f"Falha ao carregar modelo banana_nanica.pt ({caminho_nanica}): {e}")

class EntradaRequisicao(BaseModel):
  imagem: str  # Base64 string do frame

class CaixaDelimitadora(BaseModel):
  x1: float
  y1: float
  x2: float
  y2: float

class ObjetoDetectadoApi(BaseModel):
  class_id: int
  class_name: str
  confidence: float
  bbox: CaixaDelimitadora

class RespostaDeteccaoApi(BaseModel):
  deteccoes: list[ObjetoDetectadoApi]

# Mapeamento de classes COCO padrão do YOLOv8 para nosso catálogo de produtos
MAPEAMENTO_PRODUTOS_COCO = {}

# Mapeamento de classes do modelo banana_prata.pt customizado
MAPEAMENTO_PRODUTOS_BANANA = {
    0: {"id_classe": 5, "nome_classe": "banana_prata"}
}

# Mapeamento de classes do modelo banana_nanica.pt customizado
MAPEAMENTO_PRODUTOS_NANICA = {
    0: {"id_classe": 6, "nome_classe": "banana_nanica"}
}

def calcular_iou(caixa_a: CaixaDelimitadora, caixa_b: CaixaDelimitadora) -> float:
  # Calcula coordenadas da área de intersecção
  inter_x1 = max(caixa_a.x1, caixa_b.x1)
  inter_y1 = max(caixa_a.y1, caixa_b.y1)
  inter_x2 = min(caixa_a.x2, caixa_b.x2)
  inter_y2 = min(caixa_a.y2, caixa_b.y2)

  largura_inter = max(0.0, inter_x2 - inter_x1)
  altura_inter = max(0.0, inter_y2 - inter_y1)
  area_inter = largura_inter * altura_inter

  # Áreas de cada caixa
  area_a = max(0.0, caixa_a.x2 - caixa_a.x1) * max(0.0, caixa_a.y2 - caixa_a.y1)
  area_b = max(0.0, caixa_b.x2 - caixa_b.x1) * max(0.0, caixa_b.y2 - caixa_b.y1)

  area_uniao = area_a + area_b - area_inter
  if area_uniao <= 0:
    return 0.0

  return area_inter / area_uniao


def suprimir_deteccoes_sobrepostas(deteccoes: list[ObjetoDetectadoApi], limiar_iou: float = 0.45) -> list[ObjetoDetectadoApi]:
  # Ordena pela maior confiança primeiro
  deteccoes_ordenadas = sorted(deteccoes, key=lambda d: d.confidence, reverse=True)
  selecionadas: list[ObjetoDetectadoApi] = []

  for deteccao in deteccoes_ordenadas:
    sobreposta = False
    for selecionada in selecionadas:
      if calcular_iou(deteccao.bbox, selecionada.bbox) > limiar_iou:
        sobreposta = True
        break
    if not sobreposta:
      selecionadas.append(deteccao)

  return selecionadas


@app.post("/api/detectar", response_model=RespostaDeteccaoApi)
async def detectar_objetos(entrada: EntradaRequisicao):
  try:
    # Decodifica imagem Base64
    dados_imagem = base64.b64decode(entrada.imagem)
    nparr = np.frombuffer(dados_imagem, np.uint8)
    imagem = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

    if imagem is None:
      raise HTTPException(status_code=400, detail="Formato de imagem inválido.")

    # Executa inferência com os modelos carregados
    resultados_coco = modelo_yolo(imagem, verbose=False, conf=0.25)

    if modelo_banana:
        resultados_banana = modelo_banana(imagem, verbose=False, conf=0.50)
    else:
        resultados_banana = []
        
    if modelo_nanica:
        resultados_nanica = modelo_nanica(imagem, verbose=False, conf=0.50)
    else:
        resultados_nanica = []
    deteccoes_filtradas = []

    altura_img, largura_img, _ = imagem.shape

    # Função auxiliar para processar resultados de qualquer modelo
    def processar_resultados(resultados, mapeamento):
      for r in resultados:
        caixas = r.boxes
        for caixa in caixas:
          classe_original = int(caixa.cls[0].item())
          confianca = float(caixa.conf[0].item())

          # Verifica se a classe está no mapeamento
          if classe_original in mapeamento:
            info_produto = mapeamento[classe_original]

            # Coordenadas em pixel
            x1_px, y1_px, x2_px, y2_px = caixa.xyxy[0].tolist()

            # Normaliza as coordenadas (0 a 1) para o frontend usar percentual
            x1 = x1_px / largura_img
            y1 = y1_px / altura_img
            x2 = x2_px / largura_img
            y2 = y2_px / altura_img

            deteccoes_filtradas.append(
                ObjetoDetectadoApi(
                    class_id=info_produto["id_classe"],
                    class_name=info_produto["nome_classe"],
                    confidence=confianca,
                    bbox=CaixaDelimitadora(x1=x1, y1=y1, x2=x2, y2=y2)
                )
            )

    # Processa detecções do modelo COCO (yolov8n.pt)
    processar_resultados(resultados_coco, MAPEAMENTO_PRODUTOS_COCO)

    # Processa detecções do modelo customizado (banana_prata.pt)
    processar_resultados(resultados_banana, MAPEAMENTO_PRODUTOS_BANANA)

    # Processa detecções do modelo customizado (banana_nanica.pt)
    processar_resultados(resultados_nanica, MAPEAMENTO_PRODUTOS_NANICA)

    # Mantém apenas a melhor detecção (no máximo 1 por vez com maior confiança)
    if deteccoes_filtradas:
      melhor_deteccao = max(deteccoes_filtradas, key=lambda d: d.confidence)
      deteccoes_finais = [melhor_deteccao]
    else:
      deteccoes_finais = []

    return RespostaDeteccaoApi(deteccoes=deteccoes_finais)

  except Exception as erro:
    raise HTTPException(status_code=500, detail=str(erro))

# Rota alternativa com mesmo comportamento para compatibilidade
@app.post("/api/detect", response_model=RespostaDeteccaoApi)
async def detectar_objetos_compatibilidade(entrada: EntradaRequisicao):
  return await detectar_objetos(entrada)

@app.get("/api/status")
async def obter_status():
  return {"status": "ativo", "modelo": "YOLOv8n"}

if __name__ == "__main__":
  import uvicorn
  print("Iniciando o servidor FastAPI...")
  uvicorn.run(app, host="0.0.0.0", port=8000)
