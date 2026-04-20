import { useEffect, useMemo, useState } from 'react';

function isClickInsideSelectors(event, selectors) {
  return selectors.some((selector) => event.target.closest(selector));
}

export function useSelectableChartItem({ resetSelectors = [] }) {
  const [hoveredItem, setHoveredItem] = useState(null);
  const [selectedItem, setSelectedItem] = useState(null);

  const displayedItem = useMemo(() => {
    return hoveredItem ?? selectedItem;
  }, [hoveredItem, selectedItem]);

  const selectorsKey = resetSelectors.join('|');

  useEffect(() => {
    function handleDocumentMouseDown(event) {
      if (isClickInsideSelectors(event, resetSelectors)) {
        return;
      }

      setHoveredItem(null);
      setSelectedItem(null);
    }

    document.addEventListener('mousedown', handleDocumentMouseDown);

    return () => {
      document.removeEventListener('mousedown', handleDocumentMouseDown);
    };
  }, [selectorsKey]);

  function handleItemMouseEnter(item) {
    setHoveredItem(item);
  }

  function handleItemMouseLeave() {
    setHoveredItem(null);
  }

  function handleItemClick(item) {
    setSelectedItem((currentSelectedItem) => {
      if (currentSelectedItem?.key === item.key) {
        return null;
      }

      return item;
    });
  }

  return {
    hoveredItem,
    selectedItem,
    displayedItem,
    setHoveredItem,
    setSelectedItem,
    handleItemMouseEnter,
    handleItemMouseLeave,
    handleItemClick,
  };
}