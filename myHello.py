# -*- coding: utf-8 -*-
from flask import Flask
app = Flask(__name__)


@app.route('/')
def index():
    return '<h1>新版本需要重新编译吗？看看中文字可不可以显示!</h1>'

@app.route('/user/<name>')
def user(name):
    s='come and see!'
    # return '<h1>"欢迎光临," %s <h1>' % name
    return '<h1>欢迎光临' + '<h1>'

if __name__ == '__main__':
    app.run('0.0.0.0')


