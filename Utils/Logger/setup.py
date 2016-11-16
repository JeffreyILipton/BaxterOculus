from distutils.core import setup  
import py2exe  
  
setup(windows=[{
        "script":'Logger.py'
        }],
      zipfile = None,
      options={"py2exe": {
                        "includes": ["sip", "PyQt4.QtGui"],
                        "dll_excludes":["QtCore4.dll","QtGui4.dll"]
                        }
              }
     ) 