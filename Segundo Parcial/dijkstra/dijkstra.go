package dijkstra

import (
	"fmt"
	"math"

	"../common"
)

// FindPath gets the costless route between one node and another
func FindPath(nodeNames []string, weights [][]float64) error {
	start, end, err := getStartAndEnd(nodeNames)
	if err != nil {
		return err
	}

	nodes := common.GetNodes(nodeNames)
	nodes[start].Label = 0

	for nodes[end].Active {
		minIndex := common.GetMinIndex(nodes)
		if !nodes[minIndex].Active {
			return fmt.Errorf("There is not a possible route for those nodes")
		}

		nodes[minIndex].Active = false

		children, indexes := common.GetAdy(&nodes[minIndex], nodes, weights[minIndex])
		for i := range children {
			newLabel := math.Min(children[i].Label, nodes[minIndex].Label+weights[minIndex][indexes[i]])

			if newLabel < children[i].Label {
				children[i].Label = newLabel
				children[i].Parent = &nodes[minIndex]
			}
		}
	}

	printResults(start, end, nodes)

	return nil
}

func getStartAndEnd(nodes []string) (int, int, error) {
	fmt.Println("These are the available nodes")
	for _, node := range nodes {
		fmt.Println(node)
	}

	startName := common.GetOption("Which one will be the start?")
	endName := common.GetOption("Which one will be the end?")

	start := common.FindInSlice(nodes, startName)
	end := common.FindInSlice(nodes, endName)

	if start == -1 || end == -1 {
		return -1, -1, fmt.Errorf("One of those nodes does not exist")
	}

	return start, end, nil
}

func printResults(start, end int, nodes []common.Node) {
	currNode := &nodes[end]

	var result string
	for currNode != nil {
		result = fmt.Sprintf("Node: %s  Label: %.2f", currNode.Name, currNode.Label)
		fmt.Println(result)

		currNode = currNode.Parent
	}
}
