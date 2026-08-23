"""
Detecta IA — Serviço de Inferência (FastAPI + YOLO)

Recebe frames em Base64, executa inferência com modelo YOLO
e retorna as detecções normalizadas para o frontend Angular.
"""

import base64
from pathlib import Path

import cv2
import numpy as np
from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from ultralytics import YOLO

# ──────────────────────────────────────────────
# Configurações
# ──────────────────────────────────────────────
DIRETORIO_BASE = Path(__file__).resolve().parent
LIMIAR_CONFIANCA = 0.35
LIMIAR_IOU = 0.45

# Mapeamento: nome cru do modelo → (id_classe, nome_classe) do catálogo
# O best.pt detecta: 0=banana_prata, 1=banana_terra
MAPA_CLASSES: dict[str, tuple[int, str]] = {
    "banana_prata":    (5, "banana_prata"),
    "prata":           (5, "banana_prata"),
    "banana_terra":    (7, "banana_da_terra"),
    "banana_da_terra": (7, "banana_da_terra"),
    "banana":          (7, "banana_da_terra"),
    "terra":           (7, "banana_da_terra"),
    "mouse":           (8, "mouse"),
}

# ──────────────────────────────────────────────
# Carregamento do modelo
# ──────────────────────────────────────────────
ORDEM_MODELOS = ["best.pt", "yolo26n.pt"]

modelo_yolo: YOLO | None = None
nome_modelo_carregado: str = "nenhum"

for nome_arquivo in ORDEM_MODELOS:
    caminho = DIRETORIO_BASE / nome_arquivo
    if caminho.exists():
        try:
            modelo_yolo = YOLO(str(caminho))
            nome_modelo_carregado = nome_arquivo
            print(f"✔ Modelo '{nome_arquivo}' carregado. Classes: {modelo_yolo.names}")
            break
        except Exception as erro:
            print(f"✘ Falha ao carregar '{nome_arquivo}': {erro}")

if modelo_yolo is None:
    print("⚠ Nenhum modelo local encontrado. Tentando download de yolo26n.pt...")
    try:
        modelo_yolo = YOLO("yolo26n.pt")
        nome_modelo_carregado = "yolo26n.pt"
        print(f"✔ Modelo 'yolo26n.pt' baixado e carregado.")
    except Exception as erro:
        print(f"✘ Erro fatal ao carregar modelo YOLO: {erro}")

# ──────────────────────────────────────────────
# App FastAPI
# ──────────────────────────────────────────────
app = FastAPI(title="Serviço de Inferência Detecta IA", version="2.0.0")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ──────────────────────────────────────────────
# Modelos de dados (Pydantic)
# ──────────────────────────────────────────────
class EntradaRequisicao(BaseModel):
    imagem: str  # Frame codificado em Base64


class CaixaDelimitadora(BaseModel):
    x1: float
    y1: float
    x2: float
    y2: float


class ObjetoDetectado(BaseModel):
    class_id: int
    class_name: str
    confidence: float
    bbox: CaixaDelimitadora


class RespostaDeteccao(BaseModel):
    deteccoes: list[ObjetoDetectado]


# ──────────────────────────────────────────────
# Funções auxiliares
# ──────────────────────────────────────────────
def decodificar_imagem(base64_str: str) -> np.ndarray:
    """Converte uma string Base64 em imagem OpenCV (BGR)."""
    dados = base64.b64decode(base64_str)
    buffer = np.frombuffer(dados, dtype=np.uint8)
    imagem = cv2.imdecode(buffer, cv2.IMREAD_COLOR)
    if imagem is None:
        raise ValueError("Não foi possível decodificar a imagem.")
    return imagem


def mapear_classe(nome_raw: str, cls_id: int) -> tuple[int, str]:
    """Retorna (id_classe, nome_classe) com base no mapa de classes."""
    chave = nome_raw.lower().strip()
    if chave in MAPA_CLASSES:
        return MAPA_CLASSES[chave]
    return cls_id, chave


def calcular_iou(a: CaixaDelimitadora, b: CaixaDelimitadora) -> float:
    """Calcula a Intersection over Union (IoU) entre duas caixas."""
    inter_x1 = max(a.x1, b.x1)
    inter_y1 = max(a.y1, b.y1)
    inter_x2 = min(a.x2, b.x2)
    inter_y2 = min(a.y2, b.y2)

    largura = max(0.0, inter_x2 - inter_x1)
    altura = max(0.0, inter_y2 - inter_y1)
    area_inter = largura * altura

    area_a = max(0.0, a.x2 - a.x1) * max(0.0, a.y2 - a.y1)
    area_b = max(0.0, b.x2 - b.x1) * max(0.0, b.y2 - b.y1)
    area_uniao = area_a + area_b - area_inter

    return area_inter / area_uniao if area_uniao > 0 else 0.0


def suprimir_sobrepostas(
    deteccoes: list[ObjetoDetectado],
    limiar: float = LIMIAR_IOU,
) -> list[ObjetoDetectado]:
    """Non-Maximum Suppression: mantém apenas as detecções de maior confiança."""
    ordenadas = sorted(deteccoes, key=lambda d: d.confidence, reverse=True)
    resultado: list[ObjetoDetectado] = []

    for det in ordenadas:
        if not any(calcular_iou(det.bbox, sel.bbox) > limiar for sel in resultado):
            resultado.append(det)

    return resultado


def executar_inferencia(imagem: np.ndarray) -> list[ObjetoDetectado]:
    """Roda o modelo YOLO sobre a imagem e retorna as detecções mapeadas."""
    if modelo_yolo is None:
        return []

    altura_img, largura_img = imagem.shape[:2]
    resultados = modelo_yolo(imagem, verbose=False, conf=LIMIAR_CONFIANCA)
    nomes = modelo_yolo.names
    deteccoes: list[ObjetoDetectado] = []

    for resultado in resultados:
        for caixa in resultado.boxes:
            cls_id = int(caixa.cls[0].item())
            confianca = float(caixa.conf[0].item())
            nome_raw = str(nomes.get(cls_id, "objeto"))

            id_classe, nome_classe = mapear_classe(nome_raw, cls_id)
            x1, y1, x2, y2 = caixa.xyxy[0].tolist()

            deteccoes.append(
                ObjetoDetectado(
                    class_id=id_classe,
                    class_name=nome_classe,
                    confidence=confianca,
                    bbox=CaixaDelimitadora(
                        x1=x1 / largura_img,
                        y1=y1 / altura_img,
                        x2=x2 / largura_img,
                        y2=y2 / altura_img,
                    ),
                )
            )

    return suprimir_sobrepostas(deteccoes) if deteccoes else []


# ──────────────────────────────────────────────
# Endpoints
# ──────────────────────────────────────────────
@app.post("/api/detectar", response_model=RespostaDeteccao)
async def detectar_objetos(entrada: EntradaRequisicao):
    """Recebe um frame Base64, executa inferência e retorna detecções."""
    try:
        imagem = decodificar_imagem(entrada.imagem)
        deteccoes = executar_inferencia(imagem)
        return RespostaDeteccao(deteccoes=deteccoes)
    except ValueError as erro:
        raise HTTPException(status_code=400, detail=str(erro))
    except Exception as erro:
        raise HTTPException(status_code=500, detail=str(erro))


@app.post("/api/detect", response_model=RespostaDeteccao)
async def detectar_objetos_alias(entrada: EntradaRequisicao):
    """Alias de compatibilidade para /api/detectar."""
    return await detectar_objetos(entrada)


@app.get("/api/status")
async def obter_status():
    """Retorna o estado atual do serviço."""
    return {
        "status": "ativo",
        "modelo_carregado": nome_modelo_carregado,
        "classes_detectadas": modelo_yolo.names if modelo_yolo else {},
    }


# ──────────────────────────────────────────────
# Execução direta
# ──────────────────────────────────────────────
if __name__ == "__main__":
    import uvicorn

    print("Iniciando o servidor FastAPI na porta 8000...")
    uvicorn.run(app, host="0.0.0.0", port=8000)
