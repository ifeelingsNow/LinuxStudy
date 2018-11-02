import os
from flask_sqlalchemy import SQLAlchemy
from flask import Flask
basedir = os.path.abspath(os.path.dirname(__file__))


db = SQLAlchemy()

# class Role(db.Model):
#     __tablename__ = 'roles'
#     id = db.Column(db.Integer, primary_key=True)
#     name = db.Column(db.String(64), unique=True)
#     default = db.Column(db.Boolean, default=False, index=True)
#     permissions = db.Column(db.Integer)
#     users = db.relationship('User', backref='role', lazy='dynamic')

class testTable(db.Model):
    __tablename__ = 'testTB'
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(64), unique=True)
    age = db.Column(db.Integer)

app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///' + os.path.join(basedir, 'data-dev.sqlite')
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = True
print(basedir)
db.init_app(app)
r='Administrator'
role = testTable.query.filter_by(name=r).first()
print(role)