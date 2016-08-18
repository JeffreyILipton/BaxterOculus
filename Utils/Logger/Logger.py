import sys
import csv
import time
from PyQt4 import QtCore, QtGui, uic
 
qtCreatorFile = "mainwindow.ui" # Enter file here.
 
Ui_MainWindow, QtBaseClass = uic.loadUiType(qtCreatorFile)

class Datalog:
    cols
    def __init__(self,n,start=0,stop=0,sucess=-1):
        self.n = n
        self.start = start
        self.stop = stop
        self.success = sucess
    def toList():
        return [self.n,self.start,self.stop,self.success]
 
class MyApp(QtGui.QMainWindow, Ui_MainWindow):
    def __init__(self):
        QtGui.QMainWindow.__init__(self)
        Ui_MainWindow.__init__(self)
        self.setupUi(self)
        self.file = None
        self.writer = None
        self.current_test = Datalog(0)

    def openLog(self,filename):
        with open(filename,"wb") as f:
            self.file = f
            self.fileEdit.setText(filename)
            self.writer = csv.writer(f)

    def closeLog(self):
        if self.file:
            self.file.close()

    def openNewLog(self):
        fname = QtGui.QFileDialog.getOpenFileName(self, 'Open file')
        self.closeLog()
        self.openLog(fname)

    def start(self):
        t = time.time()
        self.current_test.start=t
        self.startButton.isEnabled(False)
        self.stopButton.isEnabled(True)

    def stop(self):
        t = time.time()
        self.current_test.start=t
        self.startButton.isEnabled(True)
        self.stopButton.isEnabled(False)


    def sucess(self):
        self.current_test.success = 1
        self.log()

    def fail(self):
        self.current_test.success = 0
        self.log()

    def log(self):
        self.writer.writerows(self.current_test.toList())
        n = self.current_test.n+1
        self.current_test = Datalog(n)
        self.testSpinBox.setValue(n)

 
if __name__ == "__main__":
    app = QtGui.QApplication(sys.argv)
    window = MyApp()
    window.show()
    sys.exit(app.exec_())