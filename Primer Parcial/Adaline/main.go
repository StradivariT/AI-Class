package main

import (
	"fmt"
	"math"
	"math/rand"
	"time"
)

//Scenario -
type Scenario struct {
	Inputs []float64
	Output float64
}

const (
	weightRange  = 10
	atFactor     = 0.2
	errTolerance = 1e-14
)

var (
	recalculations int
	meanSquareErr  float64
	gradient       float64
	bias           float64
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
}

func caseOutput(scenario Scenario, weights []float64) float64 {
	var output float64

	for i, input := range scenario.Inputs {
		output += input * weights[i]
	}

	return output + bias
}

func getConfig() ([]Scenario, []float64) {
	bias = randomFloat(weightRange)

	return []Scenario{
			{
				Inputs: []float64{0, 0, 0, 0},
				Output: 0,
			},
			{
				Inputs: []float64{0, 0, 0, 1},
				Output: 1,
			},
			{
				Inputs: []float64{0, 0, 1, 0},
				Output: 2,
			},
			{
				Inputs: []float64{0, 0, 1, 1},
				Output: 3,
			},
			{
				Inputs: []float64{0, 1, 0, 0},
				Output: 4,
			},
			{
				Inputs: []float64{0, 1, 0, 1},
				Output: 5,
			},
			{
				Inputs: []float64{0, 1, 1, 0},
				Output: 6,
			},
			{
				Inputs: []float64{0, 1, 1, 1},
				Output: 7,
			},
			{
				Inputs: []float64{1, 0, 0, 0},
				Output: 8,
			},
			{
				Inputs: []float64{1, 0, 0, 1},
				Output: 9,
			},
		}, []float64{
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
