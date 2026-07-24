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

# Carrega o modelo customizado mouse.pt (treinado para detectar mouse de computador)
caminho_mouse = DIRETORIO_BASE / "mouse.pt"
try:
    modelo_mouse = YOLO(str(caminho_mouse))
    print(f"Modelo mouse.pt carregado com classes: {modelo_mouse.names}")
except Exception as e:
    modelo_mouse = None
    print(f"Falha ao carregar modelo mouse.pt ({caminho_mouse}): {e}")

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
# Isso permite testar com objetos comuns da casa sem treinar um modelo novo:
# - Garrafa (ID 39) -> Água Mineral Gás
# - Maçã (ID 47) ou Laranja (ID 49) ou Banana (ID 46) -> Mirtilos Orgânicos
# - Copo (ID 41) ou Livro (ID 73) -> Leite Integral 1L
MAPEAMENTO_PRODUTOS_COCO = {
    0: {"id_classe": 4, "nome_classe": "pessoa_gabriel"},
    39: {"id_classe": 2, "nome_classe": "agua_mineral_gas"},
    47: {"id_classe": 1, "nome_classe": "mirtilos_organicos"},
    49: {"id_classe": 1, "nome_classe": "mirtilos_organicos"},
    46: {"id_classe": 1, "nome_classe": "mirtilos_organicos"},
    41: {"id_classe": 0, "nome_classe": "leite_integral_1l"},
    73: {"id_classe": 0, "nome_classe": "leite_integral_1l"}
}

# Mapeamento de classes do modelo mouse.pt customizado
MAPEAMENTO_PRODUTOS_MOUSE = {
    0: {"id_classe": 3, "nome_classe": "mouse_computador"}
}

@app.post("/api/detectar", response_model=RespostaDeteccaoApi)
async def detectar_objetos(entrada: EntradaRequisicao):
  try:
    # Decodifica imagem Base64
    dados_imagem = base64.b64decode(entrada.imagem)
    nparr = np.frombuffer(dados_imagem, np.uint8)
    imagem = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

    if imagem is None:
      raise HTTPException(status_code=400, detail="Formato de imagem inválido.")

    # Executa inferência com ambos os modelos
    # Executa inferência com ambos os modelos
    resultados_coco = modelo_yolo(imagem, verbose=False, conf=0.25)
    if modelo_mouse:
        resultados_mouse = modelo_mouse(imagem, verbose=False, conf=0.20)
    else:
        resultados_mouse = []
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

    # Processa detecções do modelo customizado (mouse.pt)
    processar_resultados(resultados_mouse, MAPEAMENTO_PRODUTOS_MOUSE)

    return RespostaDeteccaoApi(deteccoes=deteccoes_filtradas)

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
