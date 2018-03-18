#[ import voxels from text ]  | [ version 1 ]
#--------------------------------------------------------------------------
#RC3 - Living Architecture | 2017

import rhinoscriptsyntax as rs
import random
from System.Windows.Forms import*

openForm = OpenFileDialog()
openForm.Filter = "txt  files(*.txt)|*.txt|All files (*.*)|*.*"
openResult = openForm.ShowDialog()

voxelstate = 0
voxelsize = 1
voxeldensitymo = 0
voxeldensityvn = 0
voxelage = 0
voxelmaxdensitymo = 0
voxelmaxdensityvn = 0
voxelmaxage = 0

voxeldensitymolayers_pts = []
voxeldensityvnlayers_pts = []
voxelagelayers_pts = []

voxeldensitymolayers_boxes = []
voxeldensityvnlayers_boxes = []
voxelagelayers_boxes = []

importmode = rs.GetInteger("Import Mode? 1-Normal, 2-MO Density, 3-VNDensity, 4-Age", 1)
if(importmode==1):
    lay0 = rs.AddLayer("VOXELPTS")
    lay1 = rs.AddLayer("ALIVE",parent="VOXELPTS")
    lay2 = rs.AddLayer("DISCONNECTED", parent="VOXELPTS")
    lay3 = rs.AddLayer("BOXES")
    lay4 = rs.AddLayer("ALIVE",parent="BOXES")
    lay5 = rs.AddLayer("DISCONNECTED", parent="BOXES")


rs.EnableRedraw(False)

if(openResult == DialogResult.OK):
    myPath = openForm.FileName
    #set file to write mode
    box = 0
    file = open(myPath,'r')
    for i, line in enumerate(file):
        line = line.strip()
        if line[0]=="v":
            linelist = line.split(" ")
            if i==0:
                tsize = voxelsize
                box = rs.AddBox([[-(tsize/2),-(tsize/2),-(tsize/2)], [(tsize/2),-(tsize/2),-(tsize/2)], [(tsize/2),(tsize/2),-(tsize/2)],[-(tsize/2),(tsize/2),-(tsize/2)],    [-(tsize/2),-(tsize/2),(tsize/2)], [(tsize/2),-(tsize/2),(tsize/2)], [(tsize/2),(tsize/2),(tsize/2)],[-(tsize/2),(tsize/2),(tsize/2)]])
                if(importmode==2):
                    lay11 = rs.AddLayer("VOXELPTS:DENSITYMO")
                    lay22 = rs.AddLayer("CUBES:DENSITYMO")
                    for j in range(0,27):
                        rs.AddLayer("DENSITYMO_PTS" + str(j),parent="VOXELPTS:DENSITYMO")
                        rs.AddLayer("DENSITYMO_CUBES" + str(j),parent="CUBES:DENSITYMO")
                if(importmode==3):
                    lay11 = rs.AddLayer("VOXELPTS:DENSITYVN")
                    lay22 = rs.AddLayer("CUBES:DENSITYVN")
                    for j in range(0,7):
                        rs.AddLayer("DENSITYVN_PTS" + str(j),parent="VOXELPTS:DENSITYVN")
                        rs.AddLayer("DENSITYVN_CUBES" + str(j),parent="CUBES:DENSITYVN")
                        
                if(importmode==4):
                    lay11 = rs.AddLayer("VOXELPTS:AGE")
                    lay22 = rs.AddLayer("CUBES:AGE")
                    
            if linelist[0]=="voxelstate":
                voxelstate = int(linelist[1])
            if linelist[0]=="voxeldensitymo":
                voxeldensitymo = int(linelist[1])
            if linelist[0]=="voxeldensityvn":
                voxeldensityvn = int(linelist[1])
            if linelist[0]=="voxelage":
                voxelage = int(linelist[1])
            
        else:
            temp = line.strip()
            temp = temp.split(",")
            centerpt = [ float(temp[0]),float(temp[1]),float(temp[2])] 
            if(importmode==1):
                if(voxelstate==1):
                    newpt = rs.AddPoint( centerpt )
                    rs.ObjectLayer(newpt,lay1)
                    tbox = rs.CopyObject(box, centerpt)
                    rs.ObjectLayer(tbox,lay4)
                if(voxelstate==-1):
                    newpt = rs.AddPoint( centerpt )
                    rs.ObjectLayer(newpt,lay2)
                    tbox = rs.CopyObject(box, centerpt)
                    rs.ObjectLayer(tbox,lay5)

            if(importmode==2):
                if(voxelstate==1):
                    newpt = rs.AddPoint( centerpt )
                    rs.ObjectLayer(newpt,"DENSITYMO_PTS" + str(voxeldensitymo))
                    tbox = rs.CopyObject(box, centerpt)
                    rs.ObjectLayer(tbox,"DENSITYMO_CUBES" + str(voxeldensitymo))

            if(importmode==3):
                if(voxelstate==1):
                    newpt = rs.AddPoint( centerpt )
                    rs.ObjectLayer(newpt,"DENSITYVN_PTS" + str(voxeldensityvn))
                    tbox = rs.CopyObject(box, centerpt)
                    rs.ObjectLayer(tbox,"DENSITYVN_CUBES" + str(voxeldensityvn))
            if(importmode==4):
                if(voxelstate==1):
                    newpt = rs.AddPoint( centerpt )
                    rs.AddLayer("AGE_PTS" + str(voxelage), parent="VOXELPTS:AGE")
                    rs.ObjectLayer(newpt,"AGE_PTS" + str(voxelage))
                    tbox = rs.CopyObject(box, centerpt)
                    rs.AddLayer("AGE_CUBES" + str(voxelage),parent="CUBES:AGE")
                    rs.ObjectLayer(tbox,"AGE_CUBES" + str(voxelage))

rs.DeleteObject(box)
#close txt file
file.close()
rs.EnableRedraw(True)