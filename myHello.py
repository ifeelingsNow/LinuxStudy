# -*- coding: utf-8 -*-
from flask import Flask, render_template
from flask_bootstrap import Bootstrap
from flask_moment import Moment
from datetime import datetime

app = Flask(__name__)
# manager = Manager(app)
bootstrap = Bootstrap(app)
moment=Moment(app)

@app.errorhandler(404)
def page_not_found(e):
    return render_template('404.html'),404

@app.errorhandler(500)
def internal_server_error(e):
    return render_template('500.html'),500

@app.route('/')
def index():
    # response = make_response('<h1>运行在了命令行上面!</h1>')
    # response.set_cookie('answer', '42')
    # return '<h1>新版本需要重新编译吗？看看中文字可不可以显示!</h1>', 400
    return render_template('index.html', current_time=datetime.utcnow())

@app.route('/user/<name>')
def user(name):
    # return '<h1>"欢迎光临," %s <h1>' % name
    s='look and see'
    b=type(name)
    return render_template('user.html', name=name)
    # return '<h1>欢迎光临,' + str(name) + '<h1>'


if __name__ == '__main__':
    app.run()

