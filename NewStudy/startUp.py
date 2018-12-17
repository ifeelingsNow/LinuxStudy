from flask_script import Manager, Shell
from scripts import app, db, Follow, User
from flask_migrate import Migrate, MigrateCommand
manager = Manager(app)
migrate = Migrate(app,db)


def make_shell_context():
    return dict(app=app, db=db, Follow=Follow, User=User)

manager.add_command("shell", Shell(make_context=make_shell_context))
manager.add_command('db', MigrateCommand)

#你这次要做的事情可以临时的写在这里
#更新用户注册时间

if __name__ == '__main__':
    manager.run()
