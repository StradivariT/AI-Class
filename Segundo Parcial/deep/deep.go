package deep

import (
	"../common"
)

// Stack -
type Stack struct {
	Nodes []common.Finder
}

// Push another element
func (s *Stack) Push(node common.Finder) {
	s.Nodes = append(s.Nodes, node)
}

// Pop the last element
func (s *Stack) Pop() common.Finder {
	length := len(s.Nodes)

	node := s.Nodes[length-1]
	s.Nodes = s.Nodes[:length-1]

	return node
}

// Empty -
func (s *Stack) Empty() bool {
	if len(s.Nodes) == 0 {
		return true
	}

	return false
}

// Search by deepness
func Search(nodes []string, weights [][]float64) error {
	err := common.BlindSearch(nodes, weights, &Stack{}, -1, -1)
	if err != nil {
		return err
	}

	return nil
}
