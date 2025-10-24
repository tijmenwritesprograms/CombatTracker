# Functional Specification Document  
## Application: D&D Combat Tracker  

---

### 1. Overview  
The **D&D Combat Tracker** is a web application designed to manage and streamline combat encounters in tabletop RPGs, primarily *Dungeons & Dragons (5e)*. It allows users to manage player characters, add monsters, track initiative and turn order, and automate descriptive combat narration using AI-assisted features.  

---

### 2. Objectives  
- Simplify combat management for DMs.  
- Automate initiative tracking and turn progression.  
- Quickly add monsters and NPCs via statblock parsing.  
- Enhance immersion with AI-generated combat narration.  

---

### 3. Core Features  

#### 3.1 Party Management  
- **Create, edit, and delete parties.**  
  - Create multiple party profiles for different campaigns
  - Edit party names
  - Delete entire parties including all characters
- **Create, edit, and delete party members (characters).**  
  - Fields: Name, Class, Level, HP (current/max), AC, Initiative Modifier, and Notes
  - Add new characters to any party
  - Edit character details inline
  - Remove characters from parties
- **Party data persistence**
  - Automatic local storage persistence using browser localStorage
  - Data persists between browser sessions
  - Auto-save on all party and character changes
  - Import/export functionality for backup and transfer
  - Clear all data option for testing/reset
- **Multiple party profiles** supported for different campaigns
- **Character validation**
  - Required fields: Name, Class, Level, HP Max, HP Current, AC
  - Level range: 1-20 (D&D 5e standard)
  - Initiative modifier range: -5 to +10
  - AC range: 1-30  

#### 3.2 Combat Setup  
- **Start New Combat:**  
  - Select an existing party or add individual adventurers.  
  - Add one or more monsters or monster groups.  
  - Manually enter or edit initiatives for all combatants.  
  - Optionally roll initiative automatically (using modifiers).  

- **Party Selection:**
  - Dropdown selector displaying all available parties with character count
  - Warning message if no parties exist with link to Party Management
  - Selected party's characters automatically added to combatant list
  - Ability to change selected party during setup

- **Add Monsters:**  
  - Enter manually via form: Name, Type, HP, AC, Initiative Modifier
  - Type field defaults to "Humanoid" for convenience
  - Full validation on all fields using data annotations
  - Monsters can be removed individually from combatant list
  - Characters from selected party cannot be removed (deselect party instead)
  - OR paste a full monster **statblock** into a textarea (future enhancement)
  - App calls the **AI Statblock Parser (LLM)** to extract relevant fields (HP, AC, attacks, damage, etc.) and prefill monster data (future enhancement)
  - Users can confirm or edit parsed data before adding (future enhancement)

- **Initiative Management:**
  - All combatants displayed in table with name, type, HP, AC, and initiative modifier
  - "Roll Initiative" button to automatically roll 1d20 + modifier for all combatants
  - Manual override available via initiative input fields for each combatant
  - Initiative values can be edited at any time during setup
  - Initiative modifier displayed with +/- sign for clarity

- **Combat Validation:**
  - "Start Combat" button disabled until at least one combatant exists
  - Warning message displayed when trying to start with no combatants
  - Reset button clears all setup data (selected party and added monsters)

- **Combat Setup Workflow:**
  1. Select a party from the dropdown (optional)
  2. Add monsters using the "Add Monster" form
  3. Review combatant list (party characters + monsters)
  4. Roll initiative for all or manually set initiative values
  5. Click "Start Combat" to proceed to combat tracker (future: navigate to tracker)  

#### 3.3 Combat Tracking  
- **Initiative Order Management:**  
  - Combat tracker displays all combatants sorted by initiative (descending order)
  - Initiative values locked once combat starts
  - Current active combatant highlighted with visual indicator (blue row and caret icon)
  - Combatants marked as unconscious/dead shown with strikethrough text
  - Display current round number at top of tracker
  
- **Combat State Persistence:**
  - Active combat state automatically saved to browser localStorage
  - Combat persists between browser sessions (page refresh maintains state)
  - Auto-save on all combat actions (turn changes, damage, healing, status changes)
  - Combat log preserved during session
  - Import/export combat state for backup
  
- **Turn Progression:**  
  - "Next Turn" button advances to next combatant in initiative order
  - "Previous Turn" button goes back to previous combatant
  - Round counter automatically increments when all combatants have acted
  - Automatically skips unconscious/dead combatants during turn progression
  - Combat log tracks every turn change with combatant name and round number
  
- **Hit Point Tracking:**  
  - Each combatant displays current HP / max HP
  - "Bloodied" badge shown when HP below 50% of max
  - "Damage" button opens modal with quick damage amounts (5/10/15/20) or custom input
  - "Heal" button opens modal with quick healing amounts (5/10/15/20) or custom input
  - HP changes immediately reflected in the combat tracker
  - HP cannot go below 0 or above max HP
  - Automatically marks combatant as "Unconscious" when HP reaches 0
  - Automatically marks combatant as "Alive" when healed from unconscious state
  - All HP changes logged to combat log with before/after values
  
- **Status Management:**
  - Three status states: Alive (green badge), Unconscious (red badge), Dead (dark badge)
  - Status automatically updated based on HP changes
  - Status changes logged to combat log
  - Unconscious/dead combatants automatically skipped during turn progression
  - Unconscious combatants can be healed to revive them
  
- **Combat Log:**
  - Displays all actions and events in reverse chronological order (newest first)
  - Log entries include: Turn changes, Damage dealt, Healing applied, Status changes
  - Each entry shows round number, timestamp, type badge, and descriptive message
  - Color-coded badges: Turn (blue), Damage (red), Heal (green), Status (yellow)
  - Log automatically scrolls to show recent entries
  - Log cleared when combat ends
  
- **Combat Controls:**
  - "End Combat" button returns to combat setup screen
  - Combat state maintained until explicitly ended
  - Navigation to other pages while combat active preserves combat state
  - Warning message shown on combat tracker if no active combat

#### 3.4 AI-Assisted Features  

##### 3.4.1 Statblock Parsing  
- **Input:** User pastes a full monster statblock (from official or homebrew source).  
- **Process:**  
  - LLM extracts:  
    - Name  
    - Type  
    - HP (max and formula)  
    - AC  
    - Attack bonuses and damage values  
    - Speed, saving throws, and special abilities (optional)  
  - Return structured JSON or prefilled form fields.  
- **Output:** User can review and confirm the monster entry before adding to combat.  

##### 3.4.2 Combat Narration  
- **Trigger:** User selects “Generate Narration” during a combatant’s turn.  
- **Input Parameters:**  
  - Combatant name  
  - Target name  
  - Attack type (melee/ranged/spell)  
  - Result (hit, miss, critical)  
  - Optional context (location, weapon, current HP of both parties)  
- **Process:**  
  - LLM generates 1–2 sentence immersive narration describing the attack outcome.  
  - Example:  
    - *Hit:* “Eldrid thrusts his windspear, piercing the ogre’s flank with a spray of blood.”  
    - *Miss:* “Gwion’s arrow whistles past the goblin’s ear, embedding in the cave wall.”  
    - *Critical:* “Breaker’s hammer crashes into the skeleton, shattering bone and scattering shards across the floor.”  

---

### 4. User Interface  

#### 4.1 Navigation
The application uses a responsive sidebar navigation with the following main sections:
- **Home**: Landing page with overview of application features and quick access to main sections
- **Party Management**: Interface for creating and managing adventuring parties
- **Combat Setup**: Interface for configuring and starting combat encounters
- **Combat Tracker**: Active combat interface with initiative tracking, HP management, and combat log
- **Data Management**: Interface for importing, exporting, and managing persistent data

The navigation is implemented using Blazor Server routing with a collapsible sidebar for mobile/tablet devices.

#### 4.2 Screens / Views  
1. **Home / Dashboard:**  
   - Welcome message and application overview
   - Quick access cards for Party Management and Combat Setup
   - Link to documentation and project information
   - Responsive card layout using Bootstrap grid system

2. **Party Management Screen:**  
   - "Create New Party" button to add new parties
   - Card-based layout displaying all parties
   - Each party card shows:
     - Party name with edit/delete buttons
     - List of characters in table format (Name, Class, Level, HP, AC, Initiative Modifier)
     - Character management buttons (Edit, Delete)
     - "Add Character" button
   - Modal form for adding/editing characters with full validation
   - Party creation/edit form with inline validation
   - Empty state message when no parties exist

3. **Combat Setup Screen:**  
   - Party selection dropdown with all available parties
   - "Add Monster" form with fields:
     - Name (required, max 100 characters)
     - Type (required, max 50 characters, defaults to "Humanoid")
     - HP (required, min 1)
     - AC (required, 1-30)
     - Initiative Modifier (optional, -5 to +10)
   - Combatants table showing:
     - Name, Type, HP, AC, Initiative Modifier (+/- prefix)
     - Initiative input field (editable)
     - Remove button for monsters (not characters)
   - Action buttons:
     - "Roll Initiative" - rolls 1d20 + modifier for all combatants
     - "Start Combat" - proceeds to combat tracker (disabled if no combatants)
     - "Reset" - clears all setup data
   - Validation messages:
     - Warning when no parties exist
     - Form validation on monster fields
     - Warning when trying to start with no combatants
   - Empty state message when no combatants added

4. **Combat Tracker Screen:**
   - **Top Controls:**
     - Round number display (e.g., "Round 3")
     - "Previous Turn" button (left arrow)
     - "Next Turn" button (right arrow)
     - "End Combat" button (returns to combat setup)
   
   - **Initiative Order Table:**
     - Displays all combatants sorted by initiative (descending)
     - Current active combatant highlighted with blue background
     - Active combatant marked with caret icon
     - Columns: Initiative, Name, Type, HP, AC, Status, Actions
     - HP displayed as "current / max" with visual badges:
       - "0 HP" badge when HP reaches 0
       - "Bloodied" badge when HP below 50% of max
     - Status badges: Alive (green), Unconscious (red), Dead (dark)
     - Unconscious/dead combatants shown with strikethrough text
   
   - **HP Management:**
     - "Damage" button opens modal for each alive combatant
     - "Heal" button available for all combatants
     - Modals provide quick action buttons (5/10/15/20) and custom input
     - HP changes immediately update in table
     - Status automatically updated based on HP changes
   
   - **Combat Log Panel:**
     - Displays all combat events in reverse chronological order
     - Entry types: Turn (blue), Damage (red), Heal (green), Status (yellow)
     - Each entry shows: Round number, Timestamp, Type badge, Message
     - Automatically scrolls to show recent activity
     - Example entries:
       - "Fighter's turn (Initiative: 15)"
       - "Goblin takes 10 damage (7 → 0 HP)"
       - "Wizard is now Unconscious!"
       - "Round 2 begins"
   
   - **Responsive Design:**
     - Two-column layout: Initiative table (left), Combat log (right)
     - Adapts to smaller screens with stacked layout
     - Modal dialogs for damage/heal actions
     - Table automatically scrolls if many combatants

5. **Data Management Screen:**
   - **Export Data:**
     - Export all party and combat data to JSON file
     - Includes automatic filename with timestamp
     - Downloads directly to user's device
   
   - **Import Data:**
     - Upload JSON file to restore data
     - Validates file format before importing
     - Replaces all current data (with warning)
     - File size limit: 10 MB
   
   - **Clear All Data:**
     - Removes all party and combat data from browser storage
     - Requires confirmation to prevent accidental deletion
     - Shows warning to export data first
   
   - **Storage Information:**
     - Displays current data statistics:
       - Number of parties
       - Total characters across all parties
       - Active combat status
       - Current combat round and combatant count
     - Shows privacy notice about local storage


#### 4.3 Design Principles
- **Responsive Layout**: Uses Bootstrap CSS framework for mobile-first responsive design
- **Accessibility**: Semantic HTML elements with proper ARIA labels
- **Consistent Theme**: D&D themed color scheme with purple/blue gradient sidebar
- **Clean Navigation**: Clear visual hierarchy with icon-based navigation items

---

### 5. System Architecture  

#### 5.1 Frontend  
- Framework: ASP.NET Core Blazor Server
- State management: Blazor component state and service-based state management
- UI Library: Bootstrap CSS framework
- Routing: Blazor Server routing with `@page` directives  

#### 5.2 Backend  
- Backend: ASP.NET Core with C#
- Data Storage: Browser localStorage via JSInterop
- Persistence: Automatic save/load for party and combat state
- Import/Export: JSON serialization for data portability
- Authentication: ASP.NET Core Identity (optional for multi-user scenarios)  

#### 5.3 AI Integration  
- LLM endpoints:  
  - `/api/parse-statblock` → Extract monster data.  
  - `/api/generate-narration` → Create dynamic narration.  
- Each endpoint uses prompt templates tailored to the D&D 5e format.  

---

### 6. Data Model (simplified)  

#### Entities  

**Party**  
- id  
- name  
- members: [Character]  

**Character**  
- id  
- name  
- class  
- level  
- hpCurrent  
- hpMax  
- ac  
- initiativeModifier  
- notes  

**Monster**  
- id  
- name  
- type  
- hp  
- ac  
- attacks: [Attack]  
- initiativeModifier  

**Combat**  
- id  
- partyId  
- monsters: [Monster]  
- combatants: [CombatantInstance]  
- round  
- turnIndex  

**CombatantInstance**  
- referenceId (character or monster id)  
- initiative  
- hpCurrent  
- status (alive/unconscious/dead)  

---

### 7. Non-Functional Requirements  
- Responsive design (usable on desktop/tablet).  
- Persistent local storage for party and combat data.
- Data persists between browser sessions.
- Fast initiative sorting and state updates (<100ms).  
- Secure LLM API usage (tokenized requests).  
- Export combat log and data to JSON.
- Import data from JSON for backup/restore.
- Graceful error handling for storage quota exceeded.

---

### 8. Future Enhancements (Optional)  
- Condition tracking (poisoned, stunned, etc.).  
- Automated dice roller integration.  
- Map or token view.  
- Voice narration output using TTS.  
- Integration with D&D Beyond or Foundry VTT.  
