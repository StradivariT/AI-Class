package main

import (
	"bufio"
	"fmt"
	"math"
	"math/rand"
	"os"
	"strconv"
	"strings"
	"time"
)

//Scenario -
type Scenario struct {
	Inputs []float64
	Output float64
}

const (
	weightRange  = 2
	atFactor     = 0.1
	errTolerance = 1.2
)

var (
	recalculations int
	meanSquareErr  float64
	gradient       float64
	bias           float64
	questions      []string
)

func main() {
	rand.Seed(time.Now().UTC().UnixNano())
	scenarios, weights := getConfig()

	printValues(weights)

	for {
		meanSquareErr = 0
		for _, scenario := range scenarios {
			caseError := scenario.Output - caseOutput(scenario, weights)
			meanSquareErr += math.Pow(caseError, 2)
		}

		meanSquareErr /= float64(len(scenarios))
		if meanSquareErr < errTolerance {
			break
		}

		for i := range weights {
			gradient = 0
			for _, scenario := range scenarios {
				gradient += scenario.Inputs[i] * (scenario.Output - caseOutput(scenario, weights))
			}

			weights[i] += gradient * atFactor
		}

		gradient = 0
		for _, scenario := range scenarios {
			gradient += scenario.Output - caseOutput(scenario, weights)
		}

		bias += gradient * atFactor
		recalculations++
	}

	printValues(weights)

	fmt.Println("The neuron has been trained, we can now evaluate any song by answering the following questions[1, 0]")
	reader := bufio.NewReader(os.Stdin)
	testSong := Scenario{Inputs: make([]float64, 0)}

	for _, question := range questions {
		fmt.Println(question)
		ans, _ := reader.ReadString('\n')
		ans = strings.Replace(ans, "\n", "", -1)

		ansF, _ := strconv.ParseFloat(ans, 64)
		testSong.Inputs = append(testSong.Inputs, ansF)
	}

	songGrade := caseOutput(testSong, weights)
	fmt.Println("The grade for the song is: ", songGrade)
}

func caseOutput(scenario Scenario, weights []float64) float64 {
	var output float64

	for i, input := range scenario.Inputs {
		output += input * weights[i]
	}

	return output + bias
}

/*
	The inputs will be determined by:
		I1: Can it transfer any emotions?
		I2: Can you feel the quality of songwriting?
		I3: Is it metal?
		I4: Does it build energy?
		I5: Does it have a good bass line?
		I6: Are the lyrics meaningfull?

	Will be grading out of 10.

	Chosen training songs are:
		1. Metallica - One 							[0, 1, 1, 1, 0, 1] -> 9.2
		2. Megadeth - Hangar 18 					[0, 1, 1, 1, 0, 0] -> 9.1
		3. Buckethead - Golden eyes 				[1, 1, 1, 0, 1, 0] -> 9.4
		4. Pearl Jam - Alive 						[1, 1, 0, 1, 1, 1] -> 9.5
		5. Skrillex - First of the year 			[0, 0, 0, 1, 1, 0] -> 4.3
		6. Molotov - Hit me							[0, 0, 0, 1, 1, 1] -> 7.7
		7. Distubed - The sound of silence  		[1, 1, 0, 1, 0, 1] -> 8.3
		8. Michael Jackson - Black or white 		[0, 1, 0, 0, 1, 1] -> 9.0
		9. Red Hot Chili Peppers - Give it away 	[0, 1, 0, 1, 1, 0] -> 8.9
		10. Nobuo Uematsu - You're not alone    	[1, 1, 0, 1, 0, 0] -> 9.6
		11. Lil Pump - Gucci gang					[0, 0, 0, 0, 0, 0] -> 1
		12. Eminem - Cleanin' out my closet     	[1, 1, 0, 0, 1, 1] -> 9.2
		13. Tool - 46 & 2 							[0, 1, 1, 1, 1, 1] -> 9.5
		14. Limp Bizkit - Re Arranged				[0, 0, 1, 1, 1, 1] -> 8.2
		15. Iron Maiden - Hallowed be thy name  	[1, 1, 1, 1, 1, 1] -> 10
		16. Ed Sheeran - Perfect					[1, 0, 0, 0, 1, 1] -> 7.5
		17. Eric Johnson - Cliffs of dover  		[1, 1, 0, 1, 0, 0] -> 8.6
		18. The Offspring - The kids aren't alright [0, 0, 0, 1, 0, 1] -> 7.9
		19. Opeth - Ghost of perdition				[0, 1, 1, 1, 1, 0] -> 8.8
		20. Gorillaz - Clint Eastwood				[0, 1, 0, 0, 1, 0] -> 8.3
*/
func getConfig() ([]Scenario, []float64) {
	bias = randomFloat(weightRange)

	questions = make([]string, 0)
	questions = append(questions, "Can the song transfer any emotions?")
	questions = append(questions, "Can you feel the quality of the songwriting?")
	questions = append(questions, "Is it metal?")
	questions = append(questions, "Does it build energy?")
	questions = append(questions, "Does it have a good bassline?")
	questions = append(questions, "Are the lyrics meaningfull?")

	return []Scenario{
			{
				Inputs: []float64{0, 1, 1, 1, 0, 1},
				Output: 9.2,
			},
			{
				Inputs: []float64{0, 1, 1, 1, 0, 0},
				Output: 9.1,
			},
			{
				Inputs: []float64{1, 1, 1, 0, 1, 0},
				Output: 9.4,
			},
			{
				Inputs: []float64{1, 1, 0, 1, 1, 1},
				Output: 9.5,
			},
			{
				Inputs: []float64{0, 0, 0, 1, 1, 0},
				Output: 4.3,
			},
			{
				Inputs: []float64{0, 0, 0, 1, 1, 1},
				Output: 7.7,
			},
			{
				Inputs: []float64{1, 1, 0, 1, 0, 1},
				Output: 8.3,
			},
			{
				Inputs: []float64{0, 1, 0, 0, 1, 1},
				Output: 9,
			},
			{
				Inputs: []float64{0, 1, 0, 1, 1, 0},
				Output: 8.9,
			},
			{
				Inputs: []float64{1, 1, 0, 1, 0, 0},
				Output: 9.6,
			},
			{
				Inputs: []float64{0, 0, 0, 0, 0, 0},
				Output: 1,
			},
			{
				Inputs: []float64{1, 1, 0, 0, 1, 1},
				Output: 9.2,
			},
			{
				Inputs: []float64{0, 1, 1, 1, 1, 1},
				Output: 9.5,
			},
			{
				Inputs: []float64{0, 0, 1, 1, 1, 1},
				Output: 8.2,
			},
			{
				Inputs: []float64{1, 1, 1, 1, 1, 1},
				Output: 10,
			},
			{
				Inputs: []float64{1, 0, 0, 0, 1, 1},
				Output: 7.5,
			},
			{
				Inputs: []float64{1, 1, 0, 1, 0, 0},
				Output: 8.6,
			},
			{
				Inputs: []float64{0, 0, 0, 1, 0, 1},
				Output: 7.9,
			},
			{
				Inputs: []float64{0, 1, 1, 1, 1, 0},
				Output: 8.8,
			},
			{
				Inputs: []float64{0, 1, 0, 0, 1, 0},
				Output: 8.3,
			},
		}, []float64{
			randomFloat(weightRange),
			randomFloat(weightRange),
			randomFloat(weightRange),
			randomFloat(weightRange),
			randomFloat(weightRange),
			randomFloat(weightRange),
		}
}

func randomFloat(multiplier float64) float64 {
	return rand.Float64() * multiplier
}

func printValues(weights []float64) {
	message := fmt.Sprintf("Recalculations = %d\nBias = %f\n", recalculations, bias)
	for i, weight := range weights {
		message += fmt.Sprintf("Weight %d = %f\n", i+1, weight)
	}

	fmt.Println(message)
}
