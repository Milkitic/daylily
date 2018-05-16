# Dragon image detection for QQ groups by sahuang, May 2018
# Not for commercial use and just for fun

# Import libraries
import cv2
import face_recognition
import imutils
import os
import numpy as np

from scipy import ndimage, misc
from skimage.measure import compare_ssim
from PIL import Image

# Function to resize image
# Returns the resized image of a particular width
def resize(imgname, basewidth):
    img = Image.open(imgname)
    wpercent = (basewidth/float(img.size[0]))
    hsize = int((float(img.size[1])*float(wpercent)))
    img = img.resize((basewidth,hsize), Image.ANTIALIAS)
    return img

# Function to compute SSIM of two images
def computeSSIM(nameA, nameB):
    imageA = cv2.imread(nameA, 0)
    imageB = cv2.imread(nameB, 0)
    score = compare_ssim(imageA,imageB)
    return score

# Function to perform face recognition
# If face is found, we try to return two tuples: left_eyebrow and right most of bottom_lip
def face_recog(imageName):
    # Load img into a numpy array
    image = face_recognition.load_image_file(imageName)

    # Find all facial features in all the faces in the image
    face_landmarks_list = face_recognition.face_landmarks(image)

    # print("I found {} face(s) in this photograph.".format(len(face_landmarks_list)))

    if len(face_landmarks_list) == 0:
        # No face found, no detection
        return (-1, -1, -1, -1)

    '''
    for face_landmarks in face_landmarks_list:
        facial_features = [
            'chin',
            'left_eyebrow',
            'right_eyebrow',
            'nose_bridge',
            'nose_tip',
            'left_eye',
            'right_eye',
            'top_lip',
            'bottom_lip'
        ]

        for facial_feature in facial_features:
            print("The {} in this face has the following points: {}".format(facial_feature, face_landmarks[facial_feature]))

        # Let's trace out each facial feature in the image with a line!
        pil_image = Image.fromarray(image)
        d = ImageDraw.Draw(pil_image)

        for facial_feature in facial_features:
            d.line(face_landmarks[facial_feature], width=5)

        pil_image.show()
    '''

    # Find coordinates
    x0 = find_extreme(face_landmarks_list[0]['left_eyebrow'])[0]
    y0 = find_extreme(face_landmarks_list[0]['left_eyebrow'])[1]
    x1 = find_extreme(face_landmarks_list[0]['right_eyebrow'])[2]
    y1 = find_extreme(face_landmarks_list[0]['bottom_lip'])[3]

    return (x0, y0, x1, y1)

# Helper function: find xmin xmax ymin ymax
def find_extreme(aList):
    xmin = ymin = 10000
    xmax = ymax = 0
    for tup in aList:
        if tup[0] > xmax:
            xmax = tup[0]
        if tup[0] < xmin:
            xmin = tup[0]
        if tup[1] > ymax:
            ymax = tup[1]
        if tup[1] < ymin:
            ymin = tup[1]
    return (xmin, ymin, xmax, ymax)

# Main function: dragon detection
# Input: templatename, image for detection
# Expected output
# 字段            类型        说明
# result         int        供参考的识别结果：0 正常，1 龙图，2 疑似图片
# confidence     double     识别为龙图的置信度，分值 0-100

def dragon_detection(templatename):
    result = 0
    confidence = 0
    result_2_count = result_0_count = 0

    # read all pictures and put them into a list
    dragon_list = []
    path = '/Users/xiaohai/resource/'
    for image_path in os.listdir(path):
        input_path = os.path.join(path, image_path)
        if image_path[0] == 'd':
            dragon_list.append(input_path)

    # Use SSIM to first find very similar pictures
    for name in dragon_list:
        imageA = cv2.imread(name, 0)
        imageB = cv2.imread(templatename, 0)
        imageB = cv2.resize(imageB, dsize=(imageA.shape[1], imageA.shape[0]), interpolation=cv2.INTER_CUBIC)
        score = compare_ssim(imageA, imageB)
        # print(score)
        confidence = score * 100
        if score > 0.67:
            return (1, confidence)
        elif score > 0.5:
            result_2_count += 1
        else:
            result_0_count += 1

    #Compare with face recognition applied
    template = face_recog(templatename)
    if template[0] == -1:
        return (0, 0)
    img2 = cv2.imread(templatename, 0)
    for name in dragon_list:
        known = face_recog(name)
        if known[0] == -1:
            continue
        # Cut name image
        img = cv2.imread(name, 0)
        crop_img = img[known[1]:known[3], known[0]:known[2]]
        temp_img = img2[template[1]:template[3], template[0]:template[2]]
        temp_img = cv2.resize(temp_img, dsize=(crop_img.shape[1], crop_img.shape[0]), interpolation=cv2.INTER_CUBIC)
        score = compare_ssim(crop_img, temp_img)
        # print(score)
        confidence = score * 100
        if score > 0.67:
            return (1, confidence)
        elif score > 0.5:
            result_2_count += 1
        else:
            result_0_count += 1

    if result_0_count < result_2_count:
        return (2, 50)
    else:
        return (0, 0)
