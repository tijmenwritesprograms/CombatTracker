# Combat Tracker Refactoring - Testing Checklist

## Date: 2024
## Status: ? All Changes Implemented - Ready for Testing

---

## Pre-Testing Verification

- [x] Build successful
- [x] All 121 existing tests passing
- [x] No compilation errors
- [x] All files saved and committed

---

## Manual Testing Checklist

### 1. Single Monster Functionality

- [ ] **Add single monster** (e.g., "Goblin")
  - Displays with correct name
  - Shows proper HP, AC, Initiative Modifier
  - No group indicator appears

- [ ] **Roll initiative for single monster**
  - Initiative value appears in table
  - Can manually edit initiative

- [ ] **Start combat with single monster**
  - Monster appears in initiative order
  - Turn indicator shows correctly
  - Can progress to monster's turn

- [ ] **Apply damage to single monster**
  - Click "Damage" button
  - Enter damage amount
  - HP reduces correctly
  - Combat log shows correct entry
  - HP bar updates visually

- [ ] **Apply healing to single monster**
  - Click "Heal" button
  - Enter healing amount
  - HP increases correctly (not above max)
  - Combat log shows correct entry

### 2. Grouped Monster Functionality

- [ ] **Add multiple monsters as group** (e.g., 3 Orcs)
  - Displays as "Orc (×3)"
  - Shows three instance numbers (#1, #2, #3)
  - Each has individual HP display

- [ ] **Roll initiative for grouped monsters**
  - All members share same initiative value
  - Initiative appears once for the group

- [ ] **Start combat with grouped monsters**
  - Group appears as single row in initiative
  - Individual HP bars for each member
  - Individual status indicators

- [ ] **Apply damage to specific group member**
  - Click "Damage" on Orc #2
  - Enter damage
  - **VERIFY: Only Orc #2 takes damage**
  - Other orcs unaffected
  - Combat log shows "Orc 2 takes X damage"

- [ ] **Apply healing to specific group member**
  - Click "Heal" on Orc #1
  - Enter healing
  - **VERIFY: Only Orc #1 heals**
  - Other orcs unaffected
  - Combat log shows "Orc 1 heals X HP"

- [ ] **Reduce group member to 0 HP**
  - Status changes to "Unconscious"
  - HP bar disappears
  - Strikethrough on name
  - Turn skipped if that member's turn

- [ ] **Kill all group members**
  - All show "Unconscious" or "Dead"
  - Entire group row gets strikethrough
  - Turn skips entire group

### 3. Quick Reference & Statblock

- [ ] **Toggle quick reference (single monster)**
  - Click chevron/arrow button
  - Quick reference panel appears below row
  - Shows AC, HP, Speed, CR
  - Shows primary actions
  - Click again to collapse

- [ ] **Toggle quick reference (grouped monster)**
  - Click chevron on group row
  - Quick reference appears
  - Shows template stats (not individual HP)
  - Clicking any group member shows same stats

- [ ] **View full statblock**
  - Click monster name
  - Modal opens with full D&D 5e statblock
  - All sections visible (Traits, Actions, etc.)
  - Close button works
  - Click background to close

### 4. Mixed Combat (Characters + Monsters)

- [ ] **Add party with characters**
  - Select party from dropdown
  - Characters appear in combatants list

- [ ] **Add mixed monsters**
  - 1 solo monster (e.g., Troll)
  - 3 grouped monsters (e.g., 3 Goblins)

- [ ] **Roll initiative**
  - Characters get individual rolls
  - Solo monster gets individual roll
  - Grouped monsters share one roll

- [ ] **Start and run combat**
  - Turn order sorted by initiative correctly
  - Characters, solo monster, and group all display properly
  - Turn progression works through all types
  - Damage/heal target correct entities

### 5. Combat Flow

- [ ] **Next turn functionality**
  - Advances to next combatant
  - Skips unconscious/dead
  - Increments round at end of initiative order
  - Combat log entry created
  - Current turn indicator moves

- [ ] **Previous turn functionality**
  - Goes back one combatant
  - Skips unconscious/dead (backwards)
  - Decrements round if at start
  - Combat log entry created

- [ ] **Keyboard shortcuts**
  - Press 'N' ? Next turn
  - Press 'P' ? Previous turn
  - Press 'D' ? Damage modal for current combatant
  - Press 'H' ? Heal modal for current combatant
  - Press 'Esc' ? Close modal
  - Press '?' ? Show keyboard help

### 6. Combat Log

- [ ] **Damage entries**
  - Shows correct combatant name
  - Shows damage amount
  - Shows HP change (before ? after)
  - Red "Damage" badge

- [ ] **Heal entries**
  - Shows correct combatant name
  - Shows healing amount
  - Shows HP change (before ? after)
  - Green "Heal" badge

- [ ] **Status change entries**
  - "X is now Unconscious"
  - "X is now Alive" (when healed)
  - Yellow "Status" badge

- [ ] **Turn entries**
  - "X's turn"
  - "Round N begins"
  - Blue "Turn" badge

- [ ] **Log ordering**
  - Most recent at top
  - Timestamp visible
  - Round number visible

### 7. Data Persistence

- [ ] **Save combat state**
  - Add monsters and start combat
  - Close browser tab
  - Reopen application
  - Combat state restored
  - Groups preserved correctly

- [ ] **Load saved combat**
  - Initiative order maintained
  - Current turn preserved
  - HP values correct
  - Combat log preserved
  - Group membership intact

### 8. Edge Cases

- [ ] **Add 10+ monsters in single group**
  - Displays correctly
  - Performance acceptable
  - All actions work

- [ ] **Mix of multiple groups**
  - 3 Orcs (Group 1)
  - 2 Trolls (Group 2)
  - 4 Goblins (Group 3)
  - Each group displays separately
  - Correct grouping maintained

- [ ] **Damage beyond max HP**
  - HP doesn't go negative
  - Shows 0 HP
  - Status changes appropriately

- [ ] **Heal beyond max HP**
  - HP doesn't exceed max
  - Stops at max value

- [ ] **Damage/heal to dead combatant**
  - Can still heal dead combatants
  - Healing revives to Alive status
  - Can't damage dead combatants (button disabled)

### 9. UI/UX Checks

- [ ] **Visual indicators**
  - Current turn highlighted (blue background)
  - Initiative indicator (arrow/icon) visible
  - HP bars color-coded (green ? yellow ? red)
  - "Bloodied" badge at 50% HP
  - "0 HP" badge when dead

- [ ] **Responsive design**
  - Desktop: All columns visible
  - Tablet: Layout adapts appropriately
  - Actions remain accessible

- [ ] **Loading states**
  - No flickering during state changes
  - Smooth transitions
  - No layout shifts

### 10. Regression Testing

- [ ] **Party management still works**
  - Can create/edit/delete parties
  - Can add/remove characters
  - Seed test party works

- [ ] **Combat setup still works**
  - Can select party
  - Can add monsters manually
  - Can set initiative manually
  - Start combat button enables correctly

- [ ] **Data management still works**
  - Export data works
  - Import data works
  - Clear data works
  - Statistics displayed

---

## Critical Bug Verification

### ? Bug: Damage/Heal Target Wrong Combatant

**Original Issue:**
- Clicking "Damage" on "Orc 2" would damage "Orc 1" or wrong combatant
- Loop indices were unstable

**How to Test:**
1. Add 3 Orcs as group
2. Start combat
3. Click "Damage" specifically on "Orc #2" button
4. Enter 5 damage
5. **VERIFY:** Only Orc #2 loses 5 HP
6. Orc #1 and #3 remain at full HP
7. Combat log says "Orc 2 takes 5 damage"

**Expected Result:** ? Correct orc takes damage every time

---

## Performance Checks

- [ ] **Large combat (20+ combatants)**
  - Page loads in <2 seconds
  - Turn progression <100ms
  - Damage/heal <100ms
  - UI remains responsive

- [ ] **Many rounds of combat**
  - Run 50+ rounds
  - Combat log doesn't slow down
  - No memory leaks visible

---

## Browser Compatibility

Test in multiple browsers:

- [ ] **Chrome/Edge** (Chromium)
  - All features work
  - No console errors

- [ ] **Firefox**
  - All features work
  - No console errors

- [ ] **Safari** (if available)
  - All features work
  - No console errors

---

## Accessibility

- [ ] **Keyboard navigation**
  - Can tab through all controls
  - Keyboard shortcuts work
  - Focus indicators visible

- [ ] **Screen reader** (if available)
  - Tables have proper headers
  - Buttons have aria-labels
  - Status changes announced

---

## Final Verification

- [ ] No console errors during testing
- [ ] No visual glitches or layout issues
- [ ] All modals open and close properly
- [ ] All buttons respond correctly
- [ ] Combat log remains readable and accurate
- [ ] Data persistence works reliably
- [ ] Application feels fast and responsive

---

## Issues Found

Document any issues discovered during testing:

### Issue 1:
- **Description:**
- **Steps to Reproduce:**
- **Expected:**
- **Actual:**
- **Severity:** Critical / High / Medium / Low

### Issue 2:
- **Description:**
- **Steps to Reproduce:**
- **Expected:**
- **Actual:**
- **Severity:** Critical / High / Medium / Low

---

## Sign-Off

**Tested By:** ___________________  
**Date:** ___________________  
**Result:** ? Pass  ? Fail  ? Pass with Issues  
**Notes:**

---

*This checklist ensures the refactoring maintains all existing functionality while fixing the identified bugs.*
