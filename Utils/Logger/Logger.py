import sys
import csv
import time
import datetime
from PyQt4 import QtCore, QtGui, uic
 
qtCreatorFile = "mainwindow.ui" # Enter file here.
 
Ui_MainWindow, QtBaseClass = uic.loadUiType(qtCreatorFile)

class Datalog:
    headers =[ "test number","start","stop","success"]
    def __init__(self,n,start=0,stop=0,sucess=-1):
        self.n = n
        self.start = start
        self.stop = stop
        self.success = sucess
    def toList(self):
        return [self.n,self.start,self.stop,self.success]
 
class MyApp(QtGui.QMainWindow, Ui_MainWindow):
    def __init__(self):
        QtGui.QMainWindow.__init__(self)
        Ui_MainWindow.__init__(self)
        self.setupUi(self)
        self.file = None
        self.writer = None
        self.current_test = Datalog(0)

        self.startButton.setEnabled(False)
        self.stopButton.setEnabled(False)
        self.sucessButton.setEnabled(False)
        self.failButton.setEnabled(False)

        name = datetime.datetime.now().strftime("%I-%M%p on %B %d, %Y") + ".csv"
        name = name.replace(":","-").replace("/","-").replace(" ","-").replace(",","-")
        
        self.openLog(name)
        if self.writer:
            self.writer.writerow(Datalog.headers)


        self.openButton.clicked.connect(self.openNewLog)
        self.closeButton.clicked.connect(self.closeLog)
        self.startButton.clicked.connect(self.start)
        self.stopButton.clicked.connect(self.stop)
        self.sucessButton.clicked.connect(self.sucess)
        self.failButton.clicked.connect(self.fail)
        self.fileEdit.editingFinished.connect(self.lineEdited)
        

    def openLog(self,filename):
        f = open(filename,"ab")
        self.startButton.setEnabled(True)
        self.file = f
        self.fileEdit.setText(filename)
        self.writer = csv.writer(f)

    def closeLog(self):
        if self.file:
            self.file.close()

    def openNewLog(self):
        fname = str(QtGui.QFileDialog.getExistingDirectory(self, "Select Directory"))
        self.closeLog()
    
    def lineEdited(self):
        self.closeLog()
        fname = self.fileEdit.text()
        self.openLog(fname)

    def start(self):
        t = time.time()
        self.current_test.start=t
        self.startButton.setEnabled(False)
        self.stopButton.setEnabled(True)
        self.sucessButton.setEnabled(False)
        self.failButton.setEnabled(False)

    def stop(self):
        t = time.time()
        self.current_test.stop=t
        self.startButton.setEnabled(True)
        self.stopButton.setEnabled(False)
        self.sucessButton.setEnabled(True)
        self.failButton.setEnabled(True)

    def sucess(self):
        self.current_test.success = 1
        self.log()

    def fail(self):
        self.current_test.success = 0
        self.log()

    

    def log(self):
        self.writer.writerow(self.current_test.toList())
        print "logging:",self.current_test.toList()
        n = self.current_test.n+1
        self.current_test = Datalog(n)
        self.testSpinBox.setValue(n)

 
if __name__ == "__main__":
    app = QtGui.QApplication(sys.argv)
    window = MyApp()
    window.show()
    sys.exit(app.exec_())