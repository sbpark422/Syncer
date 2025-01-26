import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import classification_report
from scipy.signal import butter, lfilter
import joblib
import matplotlib.pyplot as plt

def bandpass_filter(data, lowcut, highcut, fs, order=4):
    nyquist = 0.5 * fs
    low = lowcut / nyquist
    high = highcut / nyquist
    b, a = butter(order, [low, high], btype='band')
    return lfilter(b, a, data, axis=0)

data_path = './experiment_data_Attention.csv'
data = pd.read_csv(data_path)
data['Timestamp'] = data['Timestamp'] - data['Timestamp'].iloc[0]
data['TimeWindow'] = (data['Timestamp'] // 1).astype(int)

numeric_cols = ['Fp1', 'Fp2', 'C3', 'C4', 'T5', 'T6', 'O1', 'O2']
raw_data = data[numeric_cols].values
alpha_filtered = bandpass_filter(raw_data, 8, 13, fs=256)
beta_filtered = bandpass_filter(raw_data, 13, 30, fs=256)
alpha_df = pd.DataFrame(alpha_filtered, columns=[f"Alpha_{col}" for col in numeric_cols])
beta_df = pd.DataFrame(beta_filtered, columns=[f"Beta_{col}" for col in numeric_cols])

alpha_df['TimeWindow'] = data['TimeWindow']
beta_df['TimeWindow'] = data['TimeWindow']
alpha_grouped = alpha_df.groupby('TimeWindow').mean().reset_index()
beta_grouped = beta_df.groupby('TimeWindow').mean().reset_index()

grouped_data = pd.merge(alpha_grouped, beta_grouped, on='TimeWindow')
labels = data.groupby('TimeWindow')['Label'].agg(lambda x: x.mode()[0]).reset_index()
data_aggregated = grouped_data.merge(labels, on='TimeWindow')

alpha_beta_cols = [col for col in data_aggregated.columns if "Alpha" in col or "Beta" in col]
X = data_aggregated[alpha_beta_cols].values
y = data_aggregated['Label'].values

attention_relax_mask = np.isin(y, ['attention', 'relax'])
X = X[attention_relax_mask]
y = y[attention_relax_mask]

if len(X) == 0:
    raise ValueError("No samples found for 'Attention' and 'Relax' classes. Please check your data.")

relax_mask = y == 'relax'
attention_mask = y == 'attention'
X_relax = X[relax_mask]
y_relax = y[relax_mask]
X_attention = X[attention_mask]
y_attention = y[attention_mask]
X_relax_augmented = np.tile(X_relax, (5, 1))
y_relax_augmented = np.tile(y_relax, 5)

X_balanced = np.vstack((X_relax_augmented, X_attention))
y_balanced = np.hstack((y_relax_augmented, y_attention))
label_encoder = LabelEncoder()
y_encoded = label_encoder.fit_transform(y_balanced)

X_train_val, X_test, y_train_val, y_test = train_test_split(X_balanced, y_encoded, test_size=0.1, random_state=42)
X_train, X_val, y_train, y_val = train_test_split(X_train_val, y_train_val, test_size=0.2222, random_state=42)  # 0.2222 비율로 나누면 전체에서 7:2:1 비율 유지

model = RandomForestClassifier(random_state=42, n_estimators=300)
model.fit(X_train, y_train)

model_path = './random_forest_model_Attention_AlphaBeta.joblib'
joblib.dump(model, model_path)
print(f"Model saved to {model_path}")

loaded_model = joblib.load(model_path)
print("Model loaded successfully.")

y_val_pred = loaded_model.predict(X_val)
print("Validation Set Evaluation")
print(classification_report(y_val, y_val_pred, target_names=label_encoder.classes_))

y_test_pred = loaded_model.predict(X_test)
print("Test Set Evaluation")
print(classification_report(y_test, y_test_pred, target_names=label_encoder.classes_))

feature_importances = loaded_model.feature_importances_
plt.bar(alpha_beta_cols, feature_importances)
plt.title('Feature Importance (Alpha and Beta Band)')
plt.xlabel('Features')
plt.ylabel('Importance')
plt.xticks(rotation=90)
plt.show()