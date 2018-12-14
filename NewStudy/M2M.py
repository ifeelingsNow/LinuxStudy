from flask import Flask
from flask_sqlalchemy import SQLAlchemy
from flask_script import Manager, Shell
import os


app=Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///' + os.path.join(os.path.dirname(__file__), 'test.sqlite')
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
manager = Manager(app)
db=SQLAlchemy(app)


referTable=db.Table('refertable',
                    db.Column('student_id', db.Integer, db.ForeignKey('students.id')),
                    db.Column('class_id', db.Integer, db.ForeignKey('classes.id')))

class Student(db.Model):
    __tablename__ = 'students'
    id=db.Column(db.Integer, primary_key=True)
    name=db.Column(db.String)
    classes=db.relationship('Class', secondary=referTable, backref=db.backref('students', lazy='dynamic'), lazy='dynamic')

class Class(db.Model):
    __tablename__ = 'classes'
    id=db.Column(db.Integer, primary_key=True)
    name=db.Column(db.String)

def make_shell_context():
    return dict(app=app, db=db, Student=Student, Class=Class)

manager.add_command("shell", Shell(make_context=make_shell_context))

@app.route('/')
def begin():
    return "first welcome!"

if __name__ == '__main__':
    manager.run()