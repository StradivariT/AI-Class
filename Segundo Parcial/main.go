package main

import (
	"fmt"
	"math/rand"
	"time"

	"./amplitude"
	"./deep"
	"./dijkstra"
	"./iterative"
	"./kruskal"
	"./limited"
	"./prim"
	"./uniform"
)

func main() {
	rand.Seed(time.Now().UnixNano())

	nodes, graph, err := getConfig()
	if err != nil {
		fmt.Println("ERROR", err)
		return
	}

	algorithm := getAlgorithm()
	switch algorithm {
	case "dijkstra":
		err = dijkstra.FindPath(nodes, graph)

	case "prim":
		err = prim.GenerateTree(nodes, graph)

	case "kruskal":
		err = kruskal.GenerateTree(nodes, graph)

	case "deep":
		err = deep.Search(nodes, graph)

	case "amplitude":
		err = amplitude.Search(nodes, graph)

	case "uniform":
		err = uniform.Search(nodes, graph)

	case "limited":
		err = limited.Search(nodes, graph)

	case "iterative":
		err = iterative.Search(nodes, graph)
	}

	if err != nil {
		fmt.Println("ERROR", err)
	}
}
