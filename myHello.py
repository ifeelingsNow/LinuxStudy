﻿# -*- coding: utf-8 -*-
from flask import Flask
from flask import make_response
from flask_script import Manager

app = Flask(__name__)
manager = Manager(app)

@app.route('/')
def index():
    response = make_response('<h1>运行在了命令行上面!</h1>')
    response.set_cookie('answer', '42')
    # return '<h1>新版本需要重新编译吗？看看中文字可不可以显示!</h1>', 400
    return response

@app.route('/user/<name>')
def user(name):
    # return '<h1>"欢迎光临," %s <h1>' % name
    s='look and see'
    b=type(name)
    return '<h1>欢迎光临,' + str(name) + '<h1>'

if __name__ == '__main__':
    manager.run()

