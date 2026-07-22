from ultralytics import YOLO
import os

def treinar():
    # 1. Carrega o modelo base leve do YOLOv8 (ele baixa sozinho se não tiver)
    modelo = YOLO('yolov8n.pt')

    # 2. Caminho para o teu ficheiro data.yaml (Ajusta o caminho se a tua pasta tiver outro nome)
    caminho_yaml = r"C:\Users\Aluno\Downloads\AAAA\dataset\data.yaml"

    print("?? A iniciar o treino do modelo do Mouse...")

    # 3. Roda o treino! 
    # epochs=50 -> Faz 50 passagens (como são poucas fotos, é super rápido)
    # imgsz=640 -> Resolução padrão das fotos
    resultados = modelo.train(
        data=caminho_yaml,
        epochs=50,
        imgsz=640,
        name='treino_mouse'
    )

    print("? Treino concluído com sucesso!")
    print("O teu novo ficheiro 'best.pt' foi guardado dentro da pasta 'runs/detect/treino_mouse/weights/'")

if __name__ == "__main__":
    treinar()