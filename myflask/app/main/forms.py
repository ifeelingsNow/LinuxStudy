from flask_wtf import FlaskForm
from wtforms import StringField, SubmitField
from wtforms.validators import DataRequired

class NameForm(FlaskForm):
    name = StringField(u'您的名字是？', validators=[DataRequired()])
    submit = SubmitField(u'确定')
