from http.server import BaseHTTPRequestHandler, HTTPServer
import logging
import json
import time
from main import FoodModel, FoodAgent

class AgentData:
    def __init__(self, position, has_food):
        self.position = position
        self.has_food = has_food

class Server(BaseHTTPRequestHandler):
    model = None 

    def _set_response(self):
        self.send_response(200)
        self.send_header('Content-type', 'text/html')
        self.end_headers()

    def do_GET(self):
        self._set_response()
        self.wfile.write("GET request for {}".format(self.path).encode('utf-8'))

    def do_POST(self):
        if self.model is None:
            # If no model instance is provided, return an error response
            self._set_response()
            self.wfile.write("No model instance provided.".encode('utf-8'))
            return

        try:
            # Extract relevant information from the model
            agent_data_list = [AgentData((agent.pos[0], agent.pos[1]), agent.has_food) for agent in self.model.schedule.agents]
            food_positions = [(x, y) for x in range(self.model.grid.width) for y in range(self.model.grid.height) if self.model.get_type(x, y) == 1]
            deposit_position = self.model.get_deposit_coordinates() if self.model.get_deposit_coordinates() is not None else (None, None)
            num_steps = self.model.step_call_count

            # Prepare the data as a dictionary
            data = {
                "agent_data": [{"position": agent_data.position, "has_food": agent_data.has_food} for agent_data in agent_data_list],
                "food_positions": food_positions,
                "deposit_position": deposit_position,
                "num_steps": num_steps,
            }

            # Convert the dictionary to JSON format
            json_data = json.dumps(data)

            # Set the response and send the JSON data
            self._set_response()
            self.wfile.write(json_data.encode('utf-8'))

        except Exception as e:
            # Handle any exceptions that might occur
            self._set_response()
            self.wfile.write(f"Error: {str(e)}".encode('utf-8'))

def run(server_class=HTTPServer, handler_class=Server, port=8585):
    logging.basicConfig(level=logging.INFO)
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    logging.info("Starting httpd...\n")

    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        pass

    httpd.server_close()
    logging.info("Stopping httpd...\n")

if __name__ == '__main__':
    from sys import argv
    
    # Create an instance of FoodModel
    model_instance = FoodModel(20, 20, 5, 47)

    # Set the model instance in the Server class
    Server.model = model_instance

    if len(argv) == 2:
        run(port=int(argv[1]))
    else:
        run()