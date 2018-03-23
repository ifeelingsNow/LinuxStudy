# -*- coding: utf-8 -*-
from flask import Flask, render_template
from flask_bootstrap import Bootstrap
from flask_moment import Moment
from flask_wtf import FlaskForm
from wtforms import StringField, SubmitField
from wtforms.validators import DataRequired

class NameForm(FlaskForm):
    name=StringField(u'请输入您的名字', validators=[DataRequired()])
    submit=SubmitField(u'提交')

app = Flask(__name__)
# manager = Manager(app)
bootstrap = Bootstrap(app)
moment=Moment(app)
app.config['SECRET_KEY'] = 'hard to guess string'

@app.errorhandler(404)
def page_not_found(e):
    return render_template('404.html'),404

@app.errorhandler(500)
def internal_server_error(e):
    return render_template('500.html'),500

@app.route('/', methods=['GET', 'POST'])
def index():
    name=None
    form=NameForm()
    if form.validate_on_submit():
        name=form.name.data
        form.name.data = ''
    return render_template('index.html', form=form, name=name)

@app.route('/user/<name>')
def user(name):
    # return '<h1>"欢迎光临," %s <h1>' % name
    s='look and see'
    b=type(name)
    return render_template('user.html', name=name)
    # return '<h1>欢迎光临,' + str(name) + '<h1>'


if __name__ == '__main__':
    app.run()

