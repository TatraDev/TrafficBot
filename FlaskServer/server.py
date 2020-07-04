from flask import Flask, render_template, redirect, url_for, request


app = Flask(__name__)

data = ['unity_data', 'bot_data']


@app.route('/TrafficBot/unity', methods=['GET'])
def get_unity_data():
    return data[0]

@app.route('/TrafficBot/unity', methods=['POST'])
def post_unity_data():
    data[0] = request.form['unity_data']
    
    return data[0]

@app.route('/TrafficBot/bot', methods=['GET'])
def get_bot_data():
    return data[1]

@app.route('/TrafficBot/bot', methods=['POST'])
def post_bot_data():
    data[1] = request.form['bot_data']
    
    return data[1]


if __name__ == '__main__':
    app.run(port='8000')
