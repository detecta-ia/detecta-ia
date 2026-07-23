# Backend Python — Detecta IA (YOLOv8 + FastAPI)

Este diretório contém o serviço de inferência em tempo real para detecção de produtos usando o modelo **YOLOv8** e **FastAPI**.

## Pré-requisitos

1. Python 3.9 ou superior instalado.
2. Câmera/Webcam ativa no computador (caso vá testar com feed de câmera real).

## Instalação das Dependências

Instale os pacotes necessários rodando o comando abaixo no terminal:

```bash
pip install fastapi uvicorn ultralytics opencv-python-headless pydantic
```

## Como Rodar o Backend

Na pasta `ApiExemplo/backend-python`, execute o seguinte comando:

```bash
python principal.py
```

O servidor iniciará em `http://localhost:8000`.

### Mapeamento de Testes

Como o modelo YOLOv8n padrão (`yolov8n.pt`) é treinado no dataset COCO (80 objetos comuns), mapeamos alguns objetos domésticos para os produtos do nosso carrinho para facilitar seus testes sem necessidade de treinar um modelo customizado:

*   **Garrafa de água / refrigerante (ID 39)** detectado pela câmera -> Mapeado para **Água Mineral Gás** no carrinho.
*   **Copo (ID 41)** ou **Livro (ID 73)** detectado pela câmera -> Mapeado para **Leite Integral 1L** no carrinho.
*   **Maçã (ID 47)**, **Laranja (ID 49)** ou **Banana (ID 46)** detectadas -> Mapeadas para **Mirtilos Orgânicos** no carrinho.
