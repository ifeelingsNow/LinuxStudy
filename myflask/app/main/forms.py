from flask_wtf import FlaskForm
from wtforms import StringField, SubmitField
from wtforms.validators import DataRequired

class NameForm(FlaskForm):
    name = StringField('your name is?', validators=[DataRequired()])
    submit = SubmitField('submit')
