from myHello import db, Role, User
db.drop_all()
db.create_all()
admin_role =Role(name='Admin')
mod_role = Role(name='Moderator')
user_role = Role(name='User')
user_john=User(username='John', role_id=1)
user_susan=User(username='Susan', role_id=3)
user_david=User(username='David', role_id=3)

db.session.add(admin_role)
db.session.add(mod_role)
db.session.add(user_role)
db.session.add(user_john)
db.session.add(user_susan)
db.session.add(user_david)

db.session.commit()