from math import *
import numpy as np



def YawFromQuat(theta_min,theta_max, quat):
    # https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
    q0,q1,q2,q3 = quat
    return atan2(2*(q0*q1+q2*q3),1-2*(q1*q1+q2*q2))

def magQ(q):
    return sqrt(q[0]*q[0] + q[1]*q[1] + q[2]*q[2] + q[3]*q[3])

def qFromT(m):
    tr = m[0,0]+m[1,1]+m[2,2]
    if(tr>0):
        print "c1"
        s = 2*sqrt(tr+1.0)
        print "s:",s
        w = 0.25*s
        x = (m[2,1] - m[1,2] )/s
        y = (m[0,2] - m[2,0] )/s
        z = (m[1,0] - m[0,1] )/s

    elif ( (m[0,0]>m[1,1]) and ( m[0,0]>m[2,2])):
        print "c2"
        s = sqrt(1+m[0,0]-m[1,1]-m[2,2])*2
        w = ( m[2,1] - m[1,2] )/s
        x = 0.25*s
        y = ( m[0,1] + m[1,0] )/s
        z = ( m[0,2] + m[2,0] )/s

    elif ( m[1,1]>m[2,2] ):
        print "c3"
        s = sqrt(1+m[1,1]-m[0,0]-m[2,2])*2
        w = ( m[0,2] - m[2,0] )/s
        x = ( m[0,1] + m[1,0] )/s
        y = 0.25*s
        z = ( m[1,2] + m[2,1] )/s

    else:
        print "c4"
        s = sqrt(1.0 + m[2,2] - m[0,0] - m[1,1])*2
        w = (m[1,0] - m[0,1])/s
        x = (m[0,2] + m[2,0])/s
        y = (m[1,2] + m[2,1])/s
        z = 0.25*s

    mag = magQ([w,x,y,z])
    if mag<0.9 or mag>1.1: print "Mag error:",mag
    w=w/mag
    x=x/mag
    y=y/mag
    z=z/mag
    return [w,x,y,z]

def tFromQ(q):
    t = np.mat(np.zeros((3,3)))
    w,x,y,z = q
    t[0,0] = 1 - 2*y*y - 2*z*z
    t[0,1] = 0 + 2*x*y - 2*w*z
    t[0,2] = 0 + 2*x*z + 2*w*y
    t[1,0] = 0 + 2*x*y + 2*w*z
    t[1,1] = 1 - 2*x*x - 2*z*z
    t[1,2] = 0 + 2*y*z - 2*w*x
    t[2,0] = 0 + 2*x*z - 2*w*y
    t[2,1] = 0 + 2*y*z + 2*w*x
    t[2,2] = 1 - 2*x*x - 2*y*y
    return t


def qInv(q):
    return [q[0],-q[1],-q[2],-q[3]]
def qMult(q,r):
    t=[0,0,0,0]
    t[0] = r[0]*q[0]-r[1]*q[1]-r[2]*q[2]-r[3]*q[3]
    t[1] = r[0]*q[1]+r[1]*q[0]-r[2]*q[3]+r[3]*q[2]
    t[2] = r[0]*q[2]+r[1]*q[3]+r[2]*q[0]-r[3]*q[1]
    t[3] = r[0]*q[3]-r[1]*q[2]+r[2]*q[1]+r[3]*q[0]
    return t

def mCross(k,i):
    u1 = k[0,0]
    u2 = k[1,0]
    u3 = k[2,0]
    v1 = i[0,0]
    v2 = i[1,0]
    v3 = i[2,0]

    s1 = u2*v3 - u3*v2
    s2 = u3*v1 - u1*v3
    s3 = u1*v2 - u2*v1
    return np.mat([[s1],[s2],[s3]])


def QuatForInverse(quat):
    tBU = np.mat([ [0,-1,0], [0,0,1], [1,0,0]])
    theta = pi
    A = np.mat([ [cos(theta),0,-sin(theta)], [0,1,0], [sin(theta),0,cos(theta)]])
    print "\nA:"
    print A
    qHB = quat
    tHB = tFromQ(qHB)
    tHU = tHB.dot(A).dot(tBU)
    Hx = tHU[:,0]
    Hy = tHU[:,1]
    Hz = tHU[:,2]
    Oz = Hz
    Oy = -Hy
    Ox = mCross(Oy,Oz)
    tOU = np.concatenate((Ox,Oy,Oz), axis=1)

    Q3 = qFromT(tOU)
    return Q3
    # tUB = np.mat([ [0,-1,0], [0,0,1], [1,0,0]])
    # qBH = quat
    # tBH = tFromQ(qBH)
    # tUH = tUB.dot(tBH)
    # Hx = tUH[:,0]
    # Hy = tUH[:,1]
    # Hz = tUH[:,2]
    # Oz = Hz
    # Oy = -Hy
    # Ox = mCross(Oy,Oz)
    # tOU = np.concatenate((Ox,Oy,Oz), axis=1)

    # Q3 = qFromT(tOU)
    # return Q3

def QuatTransform(quat):
    tBU = np.mat([ [0,0,1], [-1,0,0], [0,1,0] ])
    #quat= [1,0,0,0]#[cos(pi/4.0),0,cos(pi/4.0),0]
    qUO = quat#qInv(quat)
    tUO = tFromQ(qUO)
    tBO = tBU.dot(tUO)
    Ox = tBO[:,0]
    Oy = tBO[:,1]
    Oz = tBO[:,2]
    Hz = Oz
    Hy = -Oy
    Hx = mCross(Hy,Hz) #-Ox
    tHB = np.concatenate((Hx,Hy,Hz), axis=1)

    Q3 = qFromT(tHB)
    #global DEBUG
    #if DEBUG:
    #    print "qUO: ",qUO
    #    print "tUO: \n",tUO
    #    print "TBO: \n",tBO
    #    print "THB: \n",tHB
    #    print "Oy:\n",Oy
    #    print "Hy:\n",Hy
    #    print "qHB: ",Q3

    return Q3 
