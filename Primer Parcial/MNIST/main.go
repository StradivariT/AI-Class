package main

import (
	"image"
	"image/color"
	"image/png"

	"io/ioutil"

	"fmt"
	"os"
)

const (
	imgToDraw     = 46 - 1
	imgSideLength = 28
	imgSize       = imgSideLength * imgSideLength
	imgIndex      = imgToDraw*imgSize + 16
	labelIndex    = imgToDraw + 8
)

var images []uint8

func main() {
	images, err := ioutil.ReadFile("train-images")
	if err != nil {
		return
	}

	labels, err := ioutil.ReadFile("train-labels")
	if err != nil {
		return
	}

	fmt.Println("Number to draw:", labels[labelIndex])

	canvas := image.NewRGBA(image.Rect(0, 0, imgSideLength, imgSideLength))

	pixelIndex := imgIndex
	for i := 0; i < imgSideLength; i++ {
		for j := 0; j < imgSideLength; j++ {
			currPixel := images[pixelIndex]
			pixelIndex++

			pixelColor := color.RGBA{currPixel, currPixel, currPixel, 255}
			canvas.Set(j, i, pixelColor)
		}
	}

	numFile, err := os.Create("number.png")
	if err != nil {
		return
	}
	defer numFile.Close()

	png.Encode(numFile, canvas)
}
