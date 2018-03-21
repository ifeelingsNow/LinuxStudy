from flask import Flask
app = Flask(__name__)


@app.route('/')
def index():
    return '<h1>看看中文字可不可以显示!</h1>'

if __name__ == '__main__':
    app.run('0.0.0.0')


