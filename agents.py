import socket
import json
import threading
import time
import random

HOST = '127.0.0.1'
PORT = 1101
GRID_SIZE = 10
UPDATE_RATE = 0.5

agents_list = []
server_socket = None
client_conn = None
lock = threading.Lock()
running = True

class Agent:
    def __init__(self, agent_id):
        self.id = agent_id
        self.x = random.randint(0, GRID_SIZE - 1)
        self.y = random.randint(0, GRID_SIZE - 1)
    
    def step(self, occupied_positions):
        moves = [
            (self.x + 1, self.y),
            (self.x - 1, self.y),
            (self.x, self.y + 1),
            (self.x, self.y - 1),
        ]
        
        valid_moves = [
            (nx, ny) for nx, ny in moves
            if 0 <= nx < GRID_SIZE and 0 <= ny < GRID_SIZE
            and (nx, ny) not in occupied_positions
        ]
        
        if valid_moves:
            self.x, self.y = random.choice(valid_moves)
    
    def to_dict(self):
        return {'id': self.id, 'x': self.x, 'y': self.y}

def send_data_to_client():
    global client_conn, agents_list
    while running:
        time.sleep(UPDATE_RATE)
        with lock:
            if client_conn:
                try:
                    agents_data = [agent.to_dict() for agent in agents_list]
                    data_json = json.dumps(agents_data)
                    message = f"{data_json}$".encode('utf-8')
                    print(f"Enviando datos: {data_json}")
                    client_conn.send(message)
                except Exception as e:
                    print(f"Error enviando datos: {e}")
                    client_conn = None

def accept_connections():
    global client_conn, server_socket
    while running:
        try:
            conn, addr = server_socket.accept()
            print(f"Cliente conectado: {addr}")
            with lock:
                client_conn = conn
            
            config = {'grid_size': GRID_SIZE, 'agent_count': len(agents_list)}
            print(f"[SERVER → UNITY] Config inicial: {config_json}")
            config_json = json.dumps(config)
            conn.send(f"{config_json}$".encode('utf-8'))
            
        except Exception as e:
            if running:
                print(f"Error en accept: {e}")

def update_agents():
    global agents_list
    while running:
        time.sleep(UPDATE_RATE)
        with lock:
            occupied = {(agent.x, agent.y) for agent in agents_list}
            
            for agent in agents_list:
                occupied.discard((agent.x, agent.y))
                agent.step(occupied)
                occupied.add((agent.x, agent.y))

def main():
    global server_socket, agents_list, running
    agents_list = [Agent(i) for i in range(3)]
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server_socket.bind((HOST, PORT))
    server_socket.listen(1)
    print(f"Servidor escuchando en {HOST}:{PORT}")
    
    accept_thread = threading.Thread(target=accept_connections, daemon=True)
    accept_thread.start()
    
    send_thread = threading.Thread(target=send_data_to_client, daemon=True)
    send_thread.start()
    
    update_thread = threading.Thread(target=update_agents, daemon=True)
    update_thread.start()
    
    print("Sistema de agentes iniciado. Esperando cliente...")
    
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("\nServidor detenido")
        running = False
        if server_socket:
            server_socket.close()


if __name__ == '__main__':
    main()