import os
from flask import Flask, render_template, session, redirect, url_for, flash
from flask_login import UserMixin
from flask_sqlalchemy import SQLAlchemy
from datetime import datetime
from flask_wtf import FlaskForm
from wtforms import SubmitField,StringField
from flask_bootstrap import Bootstrap
from wtforms.validators import DataRequired, Length, Email, Regexp, ValidationError

basedir = os.path.abspath(os.path.dirname(__file__))

app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] =\
    'sqlite:///' + os.path.join(basedir, 'test.sqlite')
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
app.config['SECRET_KEY'] = os.environ.get('SECRET_KEY') or 'hard to guess string'

db = SQLAlchemy(app)
bootstrap=Bootstrap(app)

class Follow(db.Model):
    __tablename__ = 'follows'
    # 被哪些人关注
    follower_id=db.Column(db.Integer, db.ForeignKey('users.id'), primary_key=True)
    # 关注了的人
    followed_id=db.Column(db.Integer, db.ForeignKey('users.id'), primary_key=True)
    timestamp = db.Column(db.DateTime, default=datetime.utcnow)

class User(UserMixin, db.Model):
    __tablename__ = 'users'
    id=db.Column(db.Integer, primary_key=True)
    name=db.Column(db.String)
    member_since = db.Column(db.DateTime(), default=datetime.utcnow)
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

    def follow(self, user):
        if not self.is_following(user):
            f = Follow(follower=self, followed=user)
            db.session.add(f)

    def unfollow(self,user):
        f=self.followed.filter_by(followed_id=user.id).first()
        if f:
            db.session.delete(f)

    def is_following(self, user):
        return self.followed.filter_by(followed_id=user.id).first() is not None
    def is_followed_by(self, user):
        return self.follower.filter_by(follower_id=user.id).first() is not None

class testForm(FlaskForm):
    name=StringField('测试功能', validators=[Length(0,64)])
    button = SubmitField('提交')

@app.route('/', methods=['GET', 'POST'])
def index():
    from random import seed
    import forgery_py
    form=testForm()
    posts = User.query.all()
    if form.validate_on_submit():
        seed()
        for post in posts:
            post.member_since=forgery_py.date.date(True)
            db.session.add(post)
        db.session.commit()
    return render_template('index.html', form=form, posts=posts)
    # return '这是主页，我来试试看'

@app.route('/testurl', methods=['GET', 'POST'])
def test1():
    urlpos=url_for('index')
    # return render_template('index.html', posts=posts)
    return urlpos
