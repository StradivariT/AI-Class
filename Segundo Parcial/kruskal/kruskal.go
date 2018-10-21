package kruskal

import (
	"fmt"
	"math"

	"../common"
)

type relation struct {
	nodeA  *common.Node
	nodeB  *common.Node
	weight float64
}

// GenerateTree -
func GenerateTree(nodeNames []string, weights [][]float64) error {
	nodes := common.GetNodes(nodeNames)

	set := make([][]*common.Node, len(nodes))
	for i := range nodes {
		set[i] = []*common.Node{&nodes[i]}
	}

	relations := []relation{}

	for len(set[0]) != len(nodes) {
		aIndex, bIndex, weight := getMinNodes(weights)

		if aIndex == bIndex {
			break
		}

		aSetIndex := findInSet(set, &nodes[aIndex])
		bSetIndex := findInSet(set, &nodes[bIndex])

		if aSetIndex == bSetIndex {
			continue
		}

		set = joinSets(set, aSetIndex, bSetIndex)

		relations = append(relations, relation{
			nodeA:  &nodes[aIndex],
			nodeB:  &nodes[bIndex],
			weight: weight,
		})
	}

	printResults(relations)

	return nil
}

func getMinNodes(weights [][]float64) (int, int, float64) {
	var aIndex, bIndex int
	minRef := math.MaxFloat64

	for i := range weights {
		for j := range weights[i] {
			if weights[i][j] == -1 {
				continue
			}

			if minRef > weights[i][j] {
				minRef = weights[i][j]

				aIndex = i
				bIndex = j
			}
		}
	}

	weights[aIndex][bIndex] = -1
	weights[bIndex][aIndex] = -1

	return aIndex, bIndex, minRef
}

func findInSet(set [][]*common.Node, node *common.Node) int {
	for i := range set {
		for j := range set[i] {
			if set[i][j] == node {
				return i
			}
		}
	}

	return -1
}

func joinSets(set [][]*common.Node, aIndex, bIndex int) [][]*common.Node {
	for i := range set[bIndex] {
		set[aIndex] = append(set[aIndex], set[bIndex][i])
	}

	return append(set[:bIndex], set[bIndex+1:]...)
}

func printResults(relations []relation) {
	for i := range relations {
		result := fmt.Sprintf("Node A: %s  Node B: %s  Weight: %.2f", relations[i].nodeA.Name, relations[i].nodeB.Name, relations[i].weight)
		fmt.Println(result)
	}
}
