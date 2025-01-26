import numpy as np
import joblib
import socket
import json
from pylsl import StreamInlet, resolve_streams
from scipy.signal import butter, lfilter

def bandpass_filter(data, lowcut, highcut, fs, order=4):
    nyquist = 0.5 * fs
    low = lowcut / nyquist
    high = highcut / nyquist
    b, a = butter(order, [low, high], btype='band')
    return lfilter(b, a, data, axis=0)

UDP_IP = "10.29.131.78"
UDP_PORT = 12346
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

model_path = './random_forest_model_Attention_AlphaBeta.joblib'
model = joblib.load(model_path)
print("Model loaded successfully.")
print("Looking for available LSL streams...")
streams = resolve_streams()

if not streams:
    print("No streams found. Please ensure your EEG device is streaming data.")
    exit()

print(f"Found {len(streams)} streams:")
for stream in streams:
    print(f"Name: {stream.name()}, Type: {stream.type()}")

target_stream = None
for stream in streams:
    if stream.name() == "openbci_eeg":
        target_stream = stream
        break

if not target_stream:
    print("No stream found with the name 'openbci_eeg'")
    exit()

inlet = StreamInlet(target_stream)
print("Connected to the stream 'openbci_eeg'. Receiving data...")

window_duration = 1
sampling_rate = 256
num_channels = 8

buffer_size = int(window_duration * sampling_rate)
data_buffer = []

print("Starting prediction...")

while True:
    sample, timestamp = inlet.pull_sample()
    if sample is not None:
        data_buffer.append(sample[:num_channels])

    if len(data_buffer) >= buffer_size:
        data_array = np.array(data_buffer[:buffer_size])

        alpha_filtered = bandpass_filter(data_array, 8, 13, fs=sampling_rate)
        alpha_power = alpha_filtered.mean(axis=0)

        beta_filtered = bandpass_filter(data_array, 13, 30, fs=sampling_rate)
        beta_power = beta_filtered.mean(axis=0)

        feature_vector = np.hstack((alpha_power, beta_power)).reshape(1, -1)

        prediction = model.predict(feature_vector)
        predicted_class = "Attention" if prediction[0] == 1 else "Relax"
        print(f"Predicted Class: {predicted_class}")

        packet = {
            "type": predicted_class,
            "data": feature_vector.flatten().tolist()
        }
        json_message = json.dumps(packet)

        sock.sendto(json_message.encode(), (UDP_IP, UDP_PORT))

        data_buffer = []
