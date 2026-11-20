
import requests
import random
import time
from datetime import datetime

API_URL = "http://localhost:5247/api/SensorReadings"

# Actual machine IDs from MySQL
MACHINE_IDS = [100, 101, 202, 330, 2220, 22220]

while True:

    print("\n========== NEW SENSOR CYCLE ==========")

    for machine_id in MACHINE_IDS:

        temperature = round(random.uniform(35, 75), 2)
        motor_current = round(random.uniform(5, 14), 2)
        vibration = round(random.uniform(1, 4), 2)

        sensor_data = {
            "machineId": machine_id,
            "temperature": temperature,
            "motorCurrent": motor_current,
            "vibration": vibration
        }

        try:
            response = requests.post(
                API_URL,
                json=sensor_data,
                timeout=5
            )

            if response.status_code == 201:
                print(
                    f"[{datetime.now()}] SUCCESS | "
                    f"Machine ID: {machine_id} | "
                    f"Temp: {temperature} C | "
                    f"Current: {motor_current} A | "
                    f"Vibration: {vibration}"
                )
            else:
                print(
                    f"API ERROR | Machine ID: {machine_id} | "
                    f"{response.status_code} | {response.text}"
                )

        except requests.exceptions.RequestException as error:
            print(f"CONNECTION ERROR | Machine ID: {machine_id} | {error}")

    # Wait 5 seconds after sending data to all machines
    time.sleep(5)


# import requests
# import random
# import time
# from datetime import datetime

# API_URL = "http://localhost:5247/api/SensorReadings"

# MACHINE_ID = 202  # Change this to your actual Machine ID

# while True:

#     temperature = round(random.uniform(35, 75), 2)
#     motor_current = round(random.uniform(5, 14), 2)
#     vibration = round(random.uniform(1, 4), 2)

# # MACHINE_ID
#     sensor_data = {
#         "machineId": MACHINE_ID,
#         "temperature": temperature,
#         "motorCurrent": motor_current,
#         "vibration": vibration
#     }

#     try:
#         response = requests.post(API_URL, json=sensor_data)

#         if response.status_code == 201:
#             print(
#                 f"[{datetime.now()}] DATA SENT SUCCESSFULLY | "
#                 f"Temperature: {temperature} °C | "
#                 f"Current: {motor_current} A | "
#                 f"Vibration: {vibration}"
#             )
#         else:
#             print("API Error:", response.status_code, response.text)

#     except requests.exceptions.RequestException as error:
#         print("Connection Error:", error)

#     time.sleep(20)
