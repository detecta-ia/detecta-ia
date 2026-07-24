# -*- coding: utf-8 -*-
from ultralytics import YOLO

def treinar():
    # 1. Carrega o modelo base leve do YOLOv8
    modelo = YOLO('yolov8n.pt')

    # 2. Caminho para o seu arquivo data.yaml
    caminho_yaml = r"C:\Users\Aluno\Downloads\AAAA\ApiExemplo\backend\src\BaseApi.Infrastructure\dataset\data.yaml"

    print("Iniciando o treino do modelo do Mouse...")

    # 3. Dispara o treinamento
    resultados = modelo.train(
        data=caminho_yaml,
        epochs=50,
        imgsz=640,
        name='treino_mouse'
    )

    print("Treino concluido com sucesso!")
    print("O seu novo arquivo 'best.pt' foi salvo em: runs/detect/treino_mouse/weights/best.pt")

if __name__ == "__main__":
    treinar()