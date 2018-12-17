from flask_script import Manager, Shell
from flask import Flask
from flask_sqlalchemy import SQLAlchemy
from datetime import datetime
import os


app=Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///' + os.path.join(os.path.dirname(__file__), 'test.sqlite')
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
db=SQLAlchemy(app)
manager = Manager(app)

class Follow:
    __tablename__ = 'follows'
    follower_id=db.Column(db.Integer, db.ForeignKey('users.id'))
    followed_id=db.Column(db.Integer, db.ForeignKey('users.id'))
    timestamp = db.Column(db.DateTime, default=datetime.utcnow)

class User:
    __tablename__ = 'users'
    id=db.Column(db.Integer, primary_key=True)
    name=db.Column(db.String)
    follow=db.relationship('Follow', foreign_keys=[Follow.follower_id], backref=db.backref('follower', lazy='joined'), lazy='dynamic', cascade='all, delete-orphan')
    followed=db.relationship('Follow', foreign_keys=[Follow.followed_id], backref=db.backref('followed', lazy='joined'), lazy='dynamic', cascade='all, delete-orphan')

    @staticmethod
    def generate_fake(count=100):
        from sqlalchemy.exc import IntegrityError
        from random import seed
        import forgery_py
        import random
        seed()

        for i in range(count):
            u=User(id=random.randint(1,200),
                   name=forgery_py.name.full_name())
            db.session.add(u)
            try:
                db.session.commit()
            except IntegrityError:
                db.session.rollback()

def make_shell_context():
    return dict(app=app, db=db, Follow=Follow, User=User)

manager.add_command("shell", Shell(make_context=make_shell_context))

@app.route('/')
def begin():
    return "first welcome!"

if __name__ == '__main__':
    manager.run()
