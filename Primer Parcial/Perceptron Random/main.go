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
			if caseError == 0 {
				continue
			}

			for i := range weights {
				weights[i] = randomFloat(-weightRange, weightRange)
			}

			bias = randomFloat(-weightRange, weightRange)

			hasError = true
			recalculations++
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
	bias = randomFloat(-weightRange, weightRange)

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
			randomFloat(-weightRange, weightRange),
			randomFloat(-weightRange, weightRange),
		}
}

func randomFloat(min, max float64) float64 {
	return min + rand.Float64()*(max-min)
}

func printValues(weights []float64) {
	message := fmt.Sprintf("Recalculations = %d\nBias = %f\n", recalculations, bias)
	for i, weight := range weights {
		message += fmt.Sprintf("Weight %d = %f\n", i+1, weight)
	}

	fmt.Println(message)
}
