package amplitude

import (
	"../common"
)

// Queue -
type Queue struct {
	Nodes []common.Finder
}

// Push another element
func (q *Queue) Push(node common.Finder) {
	q.Nodes = append(q.Nodes, node)
}

// Pop the first element
func (q *Queue) Pop() common.Finder {
	node := q.Nodes[0]
	q.Nodes = q.Nodes[1:]

	return node
}

// Empty -
func (q *Queue) Empty() bool {
	if len(q.Nodes) == 0 {
		return true
	}

	return false
}

// Search by amplitude
func Search(nodes []string, weights [][]float64) error {
	err := common.BlindSearch(nodes, weights, &Queue{}, -1, -1)
	if err != nil {
		return err
	}

	return nil
}
