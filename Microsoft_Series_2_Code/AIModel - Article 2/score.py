from cmath import log
import logging
import os
import json
import mlflow
from io import StringIO
from mlflow.pyfunc.scoring_server import infer_and_parse_json_input, predictions_to_json

def init():
    """
    This function is called when the container is initialized/started, typically after create/update of the deployment.
    You can write the logic here to perform init operations like caching the model in memory
    """
    global model
    global input_schema
    # AZUREML_MODEL_DIR is an environment variable created during deployment.
    # It is the path to the model folder (./azureml-models/$MODEL_NAME/$VERSION)
    # Please provide your model's folder name if there is one
    model_path = os.getenv("AZUREML_MODEL_DIR") + "/ReviewSentiment"
    model = mlflow.pyfunc.load_model(model_path)
    input_schema = model.metadata.get_input_schema()
    logging.info("------------------------------------- Init complete -------------------------------------")
    

def run(raw_data):
    json_data = json.loads(raw_data)
    logging.info("JSON Data:", json_data)
    logging.info("---------------------------------------------------------")
    
    if "review" not in json_data.keys():
        raise Exception("Request must contain a top level key named 'review'")

    logging.warn("---------------------------------------------------------")
    logging.warn(json_data)
    logging.warn("---------------------------------------------------------")

    serving_input = { "inputs": json_data["review"] }
    data = infer_and_parse_json_input(serving_input, input_schema)
    predictions = model.predict(data)
    
    result = StringIO()
    predictions_to_json(predictions, result)
    return predictions #result.getvalue()