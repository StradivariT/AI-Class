package main

import (
	"fmt"
	"math/rand"
	"time"
)

//Scenario -
type Scenario struct {
	Inputs []float64
	Output float64
}

const (
	atFactor    = 0.2
	weightRange = 5
)

var (
	bias           float64
	recalculations int
)

func main() {
	rand.Seed(time.Now().UTC().UnixNano())
	scenarios, weights := getConfig()

	printValues(weights)

	for {
		var hasError bool

		for _, scenario := range scenarios {
			var step float64
			if caseOutput(scenario, weights) >= 0 {
				step = 1
			}

			caseError := scenario.Output - step
			if caseError != 0 {
				hasError = true
				recalculations++
			}

			for i := range weights {
				weights[i] += (caseError * atFactor * scenario.Inputs[i])
			}

			bias += (caseError * atFactor)
		}

		if !hasError {
			break
		}
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
				Inputs: []float64{0, 0},
				Output: 0,
			},
			{
				Inputs: []float64{0, 1},
				Output: 0,
			},
			{
				Inputs: []float64{1, 0},
				Output: 0,
			},
			{
				Inputs: []float64{1, 1},
				Output: 1,
			},
		}, []float64{
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
