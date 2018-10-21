package prim

import (
	"fmt"
	"math"

	"../common"
)

// GenerateTree wil generate the minimun tree starting from a random node
func GenerateTree(nodeNames []string, weights [][]float64) error {
	root := common.Random(0, len(nodeNames))

	nodes := common.GetNodes(nodeNames)
	nodes[root].Label = 0

	for stillActive(nodes) {
		minIndex := common.GetMinIndex(nodes)
		if !nodes[minIndex].Active {
			break
		}

		nodes[minIndex].Active = false

		children, indexes := common.GetAdy(&nodes[minIndex], nodes, weights[minIndex])
		for i := range children {
			if weights[minIndex][indexes[i]] < children[i].Label {
				children[i].Label = weights[minIndex][indexes[i]]
				children[i].Parent = &nodes[minIndex]
			}
		}
	}

	printResults(nodes)

	return nil
}

func stillActive(nodes []common.Node) bool {
	for i := range nodes {
		if nodes[i].Active {
			return true
		}
	}

	return false
}

func printResults(nodes []common.Node) {
	for i := range nodes {
		result := fmt.Sprintf("Node: %s  ", nodes[i].Name)

		if nodes[i].Label == math.MaxFloat64 {
			continue
		}

		if nodes[i].Parent == nil {
			result += "This is the root node!"
		} else {
			result += fmt.Sprintf("Parent: %s  Weight: %.2f", nodes[i].Parent.Name, nodes[i].Label)
		}

		fmt.Println(result)
	}
}
