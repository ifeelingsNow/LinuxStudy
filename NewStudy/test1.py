import math
class Vertex(object):

    def __init__(self, x = 0, y = 0, z = 0):
        self.x = x
        self.y = y
        self.z = z

    @staticmethod
    def Add(vt1, vt2):
        return Vertex(vt1.x + vt2.x, vt1.y + vt2.y, vt1.z + vt2.z)

    @staticmethod
    def twoPtDis(vt1, vt2):
        return math.sqrt((vt1.x - vt2.x) * (vt1.x - vt2.x) + (vt1.y - vt2.y) * (vt1.y - vt2.y) + (vt1.z - vt2.z) * (vt1.z - vt2.z))
