from flask import Flask, jsonify, request

from main import FoodModel, FoodAgent

app = Flask(__name__)

model = FoodModel(20, 20, 5, 47)
current_step = 0
all_steps_data = []  # Accumulate data for all steps

class AgentData:
    def __init__(self, agent_id, position, has_food):
        self.agent_id = agent_id
        self.position = position
        self.has_food = has_food

@app.route("/", methods=["GET", "POST"])
def handle_request():
    global current_step, all_steps_data

    if request.method == "GET":
        return jsonify({"message": "Use POST request to simulate steps."})
    
    elif request.method == "POST":
        while current_step < 805:
            model.step()
            data = extract_model_data(model)
            all_steps_data.append(data)
            current_step += 1

        # Simulation completed, send all data
        return jsonify({"all_steps_data": all_steps_data, "message": "Simulation completed"})

def extract_model_data(model):
    agent_data_list = [AgentData(agent.unique_id, (agent.pos[0], agent.pos[1]), agent.has_food) for agent in model.schedule.agents]
    food_positions = [(x, y) for x in range(model.grid.width) for y in range(model.grid.height) if model.get_type(x, y) == 1]
    deposit_position = model.get_deposit_coordinates() if model.get_deposit_coordinates() is not None else (None, None)
    num_steps = model.step_call_count

    return {
        "agent_data": [{"id": agent_data.agent_id, "position": agent_data.position, "has_food": agent_data.has_food} for agent_data in agent_data_list],
        "food_positions": food_positions,
        "deposit_position": deposit_position,
        "num_steps": num_steps,
    }

if __name__ == "__main__":
    port = 8585
    app.run(debug=True, port=port)