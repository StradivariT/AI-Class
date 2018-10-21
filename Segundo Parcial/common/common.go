package common

import (
	"bufio"
	"fmt"
	"math"
	"math/rand"
	"os"
	"strings"
)

// Node -
type Node struct {
	Name   string
	Label  float64
	Active bool
	Parent *Node
}

// GetOption will return an input by the user based on the message
func GetOption(msg string) string {
	fmt.Println(msg)

	reader := bufio.NewReader(os.Stdin)

	option, _ := reader.ReadString('\n')
	option = strings.Replace(option, "\n", "", -1)

	fmt.Println("")

	return option
}

// FindInSlice will return the index the element was found, -1 otherwise
func FindInSlice(slice []string, elem string) int {
	for i := range slice {
		if slice[i] == elem {
			return i
		}
	}

	return -1
}

// Random will generate a random int within a range
func Random(min, max int) int {
	return rand.Intn(max-min) + min
}

// GetNodes will return an array of nodes initialized for Dijkstra and Prim
func GetNodes(nodeNames []string) []Node {
	nodes := make([]Node, len(nodeNames))
	for i := range nodes {
		nodes[i].Name = nodeNames[i]
		nodes[i].Label = math.MaxFloat64
		nodes[i].Active = true
	}

	return nodes
}

// GetMinIndex will return the index of the node with the minimun label for Dijkstra and Prim
func GetMinIndex(nodes []Node) int {
	minRef := math.MaxFloat64

	var minIndex int
	for i := range nodes {
		if nodes[i].Active && minRef > nodes[i].Label {
			minRef = nodes[i].Label
			minIndex = i
		}
	}

	return minIndex
}

// GetAdy will return the adyacent nodes to a reference for Dijkstra and Prim
func GetAdy(parent *Node, nodes []Node, adyWeights []float64) ([]*Node, []int) {
	children := []*Node{}
	indexes := []int{}

	for i := range adyWeights {
		if adyWeights[i] == -1 || !nodes[i].Active {
			continue
		}

		children = append(children, &nodes[i])
		indexes = append(indexes, i)
	}

	return children, indexes
}
