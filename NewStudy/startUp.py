from flask_script import Manager, Shell
from scripts import app, db, Follow, User
manager = Manager(app)


def make_shell_context():
    return dict(app=app, db=db, Follow=Follow, User=User)

manager.add_command("shell", Shell(make_context=make_shell_context))
#你这次要做的事情可以临时的写在这里


if __name__ == '__main__':
    manager.run()
