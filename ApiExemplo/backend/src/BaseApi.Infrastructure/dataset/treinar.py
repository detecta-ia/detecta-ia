import os
import yaml
from ultralytics import YOLO

def treinar_detector():
    # Como o script ja esta dentro de .../dataset, a raiz dos dados e a propria pasta atual ('.')
    caminho_base = os.path.abspath(".")
    
    config_dados = {
        "path": caminho_base,
        "train": "train/images",
        "val": "train/images",  
        "nc": 1,
        "names": {0: "mouse"}
    }
    
    caminho_yaml = os.path.abspath("./data.yaml")
    with open(caminho_yaml, "w") as f:
        yaml.dump(config_dados, f, default_flow_style=False)
    
    print("Arquivo data.yaml gerado automaticamente e corrigido.")
    print("Carregando modelo...")
    model = YOLO("yolov8n.pt")
    
    print("Iniciando o treino...")
    model.train(
        data=caminho_yaml,   
        epochs=10,        
        imgsz=640,
        device="cpu"      
    )
    print("Treino finalizado!")

if __name__ == "__main__":
    treinar_detector()