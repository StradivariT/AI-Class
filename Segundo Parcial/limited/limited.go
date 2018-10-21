package limited

import (
	"strconv"

	"../common"
	"../deep"
)

// Search by limited deepness
func Search(nodes []string, weights [][]float64) error {
	limit, err := GetLimit()

	err = common.BlindSearch(nodes, weights, &deep.Stack{}, limit, -1)
	if err != nil {
		return err
	}

	return nil
}

// GetLimit -
func GetLimit() (int64, error) {
	option := common.GetOption("What would the limit be?")
	limit, err := strconv.ParseInt(option, 10, 64)
	if err != nil {
		return -1, err
	}

	return limit, nil
}
