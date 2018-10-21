package main

import (
	"io/ioutil"
	"strconv"
	"strings"

	"./common"
)

const (
	nodeSeparator   = ";"
	weightSeparator = ","
)

var (
	graphs = map[string]string{
		"1": "ryanair",
		"2": "pentagon",
		"3": "big",
		"4": "maze",
	}

	ryanGraphs = map[string]string{
		"1": "availabilities.txt",
		"2": "distances.txt",
		"3": "prices.txt",
	}

	algorithms = map[string]string{
		"1": "dijkstra",
		"2": "prim",
		"3": "kruskal",
		"4": "deep",
		"5": "amplitude",
		"6": "uniform",
		"7": "limited",
		"8": "iterative",
	}
)

func getConfig() ([]string, [][]float64, error) {
	msg := "Which graph would you like to test?\n" +
		"1. Ryanair\n" +
		"2. Pentagon\n" +
		"3. Big\n" +
		"4. Maze"

	option := common.GetOption(msg)

	var nodes []string
	var weights [][]float64
	var err error
	switch graphs[option] {
	case "ryanair":
		nodes, err = getNodes("ryanair")
		weights, err = getRyanGraph()

	case "maze":
		weights, err = getGraph("maze/map.txt")

	default:
		nodes, err = getNodes(graphs[option])
		weights, err = getGraph(graphs[option] + "/weights.txt")
	}

	if err != nil {
		return nil, nil, err
	}

	return nodes, weights, nil
}

func getNodes(option string) ([]string, error) {
	nodesBytes, err := ioutil.ReadFile("graphs/" + option + "/nodes.txt")
	if err != nil {
		return nil, err
	}

	return strings.Split(string(nodesBytes), nodeSeparator), nil
}

func getGraph(filename string) ([][]float64, error) {
	weightsBytes, err := ioutil.ReadFile("graphs/" + filename)
	if err != nil {
		return nil, err
	}

	weightsByNodes := strings.Split(string(weightsBytes), nodeSeparator)
	weights := make([][]float64, len(weightsByNodes))

	for i := range weightsByNodes {
		weightsString := strings.Split(weightsByNodes[i], weightSeparator)
		weights[i] = make([]float64, len(weightsString))

		for j := range weightsString {
			weights[i][j], err = strconv.ParseFloat(weightsString[j], 64)
			if err != nil {
				return nil, err
			}
		}
	}

	return weights, nil
}

func getRyanGraph() ([][]float64, error) {
	msg := "Which graph of Ryanair would you like to use?\n" +
		"1. Availabilities\n" +
		"2. Distances\n" +
		"3. Prices"

	option := common.GetOption(msg)

	return getGraph("ryanair/" + ryanGraphs[option])
}

func getAlgorithm() string {
	msg := "Which algorithm would you like to use?\n" +
		"1. Dijkstra\n" +
		"2. Prim\n" +
		"3. Kruskal\n" +
		"4. Deep\n" +
		"5. Amplitude\n" +
		"6. Uniform\n" +
		"7. Limited\n" +
		"8. Iterative"

	option := common.GetOption(msg)

	return algorithms[option]
}
