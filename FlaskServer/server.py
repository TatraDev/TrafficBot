from flask import Flask, render_template, redirect, url_for, request

app = Flask(__name__)

unity_data = []
bot_data = []


@app.route('/TrafficBot/unity', methods=['GET'])
def get_unity_data():
    if len(unity_data) > 0:
        data = unity_data.pop(0)
    else:
        data = 'null'

    return data


@app.route('/TrafficBot/unity', methods=['POST'])
def post_unity_data():
    r = request.form['unity_data']
    if len(unity_data) > 0:
        unity_data.pop(0)
        unity_data.append(r)
    else:
        unity_data.append(r)

    return r


@app.route('/TrafficBot/bot', methods=['GET'])
def get_bot_data():
    if len(bot_data) > 0:
        data = bot_data.pop(0)
    else:
        data = 'null'

    return data


@app.route('/TrafficBot/bot', methods=['POST'])
def post_bot_data():
    r = request.form['bot_data']
    if len(bot_data) > 0:
        bot_data.pop(0)
        bot_data.append(r)
    else:
        bot_data.append(r)

    return r


if __name__ == '__main__':
    app.run(port='8000')
