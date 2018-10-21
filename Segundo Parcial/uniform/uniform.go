package uniform

import (
	"math"

	"../common"
)

// Frontier data structure with different ways of Pushing and Popping
type Frontier struct {
	Nodes []common.Finder
}

// Push checks if the node to insert exists or not, if it does then replaces if the label is lower,
// if the node doesn't exist, then it gets inserted
func (f *Frontier) Push(node common.Finder) {
	if f.Empty() {
		f.Nodes = append(f.Nodes, node)
		return
	}

	for i := range f.Nodes {
		if f.Nodes[i].ID() != node.ID() {
			continue
		}

		if f.Nodes[i].GetLabel() < node.GetLabel() {
			return
		}

		f.Nodes[i] = node
		return
	}

	f.Nodes = append(f.Nodes, node)
}

// Pop returns the node with the lower value
func (f *Frontier) Pop() common.Finder {
	minRef := math.MaxFloat64

	var minIndex int
	for i := range f.Nodes {
		if minRef > f.Nodes[i].GetLabel() {
			minRef = f.Nodes[i].GetLabel()
			minIndex = i
		}
	}

	node := f.Nodes[minIndex]
	f.Nodes = append(f.Nodes[:minIndex], f.Nodes[minIndex+1:]...)

	return node
}

// Empty -
func (f *Frontier) Empty() bool {
	if len(f.Nodes) == 0 {
		return true
	}

	return false
}

// Search by uniform cost
func Search(nodes []string, weights [][]float64) error {
	err := common.BlindSearch(nodes, weights, &Frontier{}, -1, -1)
	if err != nil {
		return err
	}

	return nil
}
