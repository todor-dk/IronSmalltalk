

Object
	subclass: #STDefItem
	instanceVariableNames: 'lines'
	classVariableNames: ''
	poolDictionaries: 'CharacterConstants'
	category: 'X3J20'!
   
STDefItem
	subclass: #STClassLibrary
	instanceVariableNames: 'protocols'
	classVariableNames: ''
	poolDictionaries: ''
	category: 'X3J20'!
 
STDefItem
	subclass: #STMessage
	instanceVariableNames: 'docSection isRefined synopsis definition refinements errors parameters names selectors synopsisLines definitionLines returnValue'
	classVariableNames: ''
	poolDictionaries: 'CharacterConstants'
	category: 'X3J20'!
 
STDefItem
	subclass: #STProtocol
	instanceVariableNames: 'name isAbstract description conformsTo messages docSection descriptionLines standardGlobalLines messageLines standardGlobals'
	classVariableNames: ''
	poolDictionaries: 'CharacterConstants'
	category: 'X3J20'!

STDefItem
	subclass: #STSpecification
	instanceVariableNames: 'protocols'
	classVariableNames: ''
	poolDictionaries: ''
	category: 'X3J20'!

STSpecification
	subclass: #STParameter
	instanceVariableNames: 'name aliasing'
	classVariableNames: ''
	poolDictionaries: 'CharacterConstants'
	category: 'X3J20'!

STSpecification
	subclass: #STReturnValue
	instanceVariableNames: 'aliasing description'
	classVariableNames: ''
	poolDictionaries: 'CharacterConstants'
	category: 'X3J20'!
   
STDefItem
	subclass: #STStandardGlobal
	instanceVariableNames: 'name description'
	classVariableNames: ''
	poolDictionaries: ''
	category: 'X3J20'!

Object
	subclass: #XItem
	instanceVariableNames: ''
	classVariableNames: ''
	poolDictionaries: ''
	category: 'X3J20'!
  
XItem
	subclass: #XDoc
	instanceVariableNames: 'pages'
	classVariableNames: ''
	poolDictionaries: ''
	category: 'X3J20'!
   
XItem
	subclass: #XFont
	instanceVariableNames: 'id size color family page'
	classVariableNames: ''
	poolDictionaries: ''
	category: 'X3J20'!
  
XItem
	subclass: #XLine
	instanceVariableNames: 'page texts top'
	classVariableNames: ''
	poolDictionaries: ''
	category: 'X3J20'!
 
XItem
	subclass: #XPage
	instanceVariableNames: 'texts number fonts document lines'
	classVariableNames: ''
	poolDictionaries: ''
	category: 'X3J20'!
  
XItem
	subclass: #XText
	instanceVariableNames: 'top left width height font text page'
	classVariableNames: ''
	poolDictionaries: ''
	category: 'X3J20'!

!STClassLibrary methodsFor: 'as yet unclassified' !

fromDoc: x3j20doc
    
    
    x3j20doc allLinesDo: [ :x3j20line | lines add: x3j20line ].!
 
initialize
    
        super initialize.
    protocols := OrderedCollection new.
!
 
processLines

	| line protocol map |
	
	lines do: [ :x3j20line |
		line := self removeTags: x3j20line asString.
		(STProtocol isProtocolStart: line) ifTrue: [
			protocol isNil ifFalse: [ protocols add: protocol ].
			protocol := STProtocol new.
		].
		protocol isNil ifFalse: [ protocol lines add: x3j20line ].
	].
	protocol isNil ifFalse: [ protocols add: protocol ].

	protocols do: [ :e | e processLines ].

	map := Dictionary new.
	map at: 'ANY' put: 'OK'.
	map at: 'RECEIVER' put: 'OK'.
	protocols do: [ :p |
		(map includesKey: p name) ifTrue: [ self error: 'Already defined' ].
		map at: p name put: p.
	].
"	map at: 'dyadicBlock' put: (map at: 'dyadicValuable')."

	protocols do: [ :p |
		p conformsTo do: [ :n |
			(map includesKey: n) ifFalse: [ self error: 'Protocol missing' ].
		].
		p messages do: [ :m |
			m definition notNil ifTrue: [
				(map includesKey: m definition key) ifFalse: [ self error: 'Protocol missing' ].
			].
			m refinements do: [ :assoc |
				(map includesKey: assoc key) ifFalse: [ self error: 'Protocol missing' ].
			].
			m parameters do: [ :pm |
				pm protocols do: [ :pt |
					(map includesKey: pt) ifFalse: [ self error: 'Protocol missing' ].
				].
			].
		].
	].!
   
protocols
    
        ^protocols.!
   
writeXmlFile: fileName

	| fs |
	fs := File newFile: fileName.
	[ 
		fs nextPutAll: '<?xml version="1.0" encoding="UTF-8" ?>'; cr.
		self writeXmlOn: fs.
	] ensure: [ fs close ].!
  
writeXmlOn: str

	str nextPutAll: '<SystemDescription>'; cr.
	self protocols do: [ :e | e writeXmlOn: str ].
	str nextPutAll: '</SystemDescription>'; cr.! !

!STClassLibrary class methodsFor: 'as yet unclassified' !
 
convert

	| doc sd |
	doc := XDoc fromFile: 'D:\Temp\X3J20.xml'.
	doc cleanUp.

	sd := self fromDoc: doc.
	sd processLines.
	sd writeXmlFile: 'd:\Temp\SystemDescription.xml'.
!

fromDoc: x3j20Doc
    
    
    ^self new fromDoc: x3j20Doc; yourself.! !

!STDefItem methodsFor: 'as yet unclassified' !
  
addLinesFrom: lns to: dstLines startingAt: start endOnSections: endOnSections
    
        ^self class addLinesFrom: lns to: dstLines startingAt: start endOnSections: endOnSections.!

addLinesFrom: lns to: dstLines startingAt: start endTest: aBlock
    
        ^self class addLinesFrom: lns to: dstLines startingAt: start endTest: aBlock.!
  
htmlDescriptionFromLines: lns

	^self class htmlDescriptionFromLines: lns.!
   
htmlDescriptionFromTexts: texts

	^self class htmlDescriptionFromTexts: texts.!
   
htmlFromLines: lns

	^self class htmlFromLines: lns.!
 
htmlFromTexts: texts

	^self class htmlFromTexts: texts.!
 
initialize
    
        lines := OrderedCollection new.!
  
lines
    
        ^lines!

removeTags: aString
    
        ^self class removeTags: aString.!

replaceHtml: aString
    
        ^self class replaceHtml: aString.!
  
toText: aCollection
     
    
	^self class toText: aCollection.!

writeXmlAttribute: name value: value on: str
	
	| s c |
	str space; nextPutAll: name; nextPutAll: '="'.
	self writeXmlText: value on: str.
	str nextPut: $".!
  
writeXmlText: value on: str
	
	| s c |
	s := value asStream.
	[ s atEnd ] whileFalse: [
		| rep |
		c := s next.
		c asciiValue >= 128 ifTrue: [ self error: 'Non ascii' ].
		c = $< ifTrue: [ rep := '&lt;' ].
		c = $> ifTrue: [ rep := '&gt;' ].
		c = $& ifTrue: [ rep := '&amp;' ].
		c = $" ifTrue: [ rep := '&quot;' ].
		rep isNil ifTrue: [ str nextPut: c ] ifFalse: [ str nextPutAll: rep ].
	].! !

!STDefItem class methodsFor: 'as yet unclassified' !
 
addLinesFrom: lines to: dstLines startingAt: start endOnSections: endOnSections
    
        ^self addLinesFrom: lines to: dstLines startingAt: start endTest: [ : str | endOnSections includes: str ].!
  
addLinesFrom: lines to: dstLines startingAt: start endTest: aBlock
    
    | line str |
    
    start to: lines size do: [ :idx |
        line := lines at: idx.
        str := line asString trimBlanks.
        (aBlock value: str)
            ifTrue: [ ^idx ]
            ifFalse: [ dstLines add: line ].
    ].
    ^lines size.!
   
htmlDescriptionFromLines: lns

	| texts |
	texts := OrderedCollection new.
	lns do: [ :l | texts addAll: l texts ].
	^self htmlDescriptionFromTexts: texts.
!
 
htmlDescriptionFromTexts: texts

	| codeBlock str lastTop paragraphs lastTopSum lastTopCnt |
	codeBlock := false.
	texts isEmpty ifTrue: [ ^nil ].
	str := WriteStream on: String new.
	lastTopSum := texts first top.
	lastTopCnt := 1.
	paragraphs := OrderedCollection new.
	(texts reject: [ :t | t text isEmpty | (t font = '15') ]) do: [ :t | 
		lastTop := lastTopSum / lastTopCnt.
		(t top - lastTop) abs <= 16 ifTrue: [
			lastTopSum := lastTopSum + t top.
			lastTopCnt := lastTopCnt + 1.
		] ifFalse: [
			(t top - lastTop) abs > 20 ifTrue: [
				paragraphs add: str contents.
				str := WriteStream on: String new.
			].
			lastTopSum := t top.
			lastTopCnt := 1.
		].

		(#('7' '18') includes: t font) ifTrue: [
			t == texts first ifFalse: [ str space ].
			codeBlock ifFalse: [ str nextPutAll: '<code>' ].
			codeBlock := true.
		] ifFalse: [ (#('1' '4' '10' '16' '3' '12' '13' '14') includes: t font) ifTrue: [
			codeBlock ifTrue: [ str nextPutAll: '</code>' ].
			codeBlock := false.
			t == texts first ifFalse: [ str space ].
		] ifFalse: [
			self error: 'Unknown font'.
		] ].

		t isItalic ifTrue: [ str nextPutAll: '<em>' ].
		str nextPutAll: t text.
		t isItalic ifTrue: [ str nextPutAll: '</em>' ].
	].
	paragraphs isEmpty ifTrue: [ ^str contents ].
	paragraphs add: str contents.
	str := WriteStream on: String new.
	paragraphs do: [ :p | str nextPutAll: '<p>'; nextPutAll: p; nextPutAll: '</p>' ].

	^str contents.!
 
htmlFromLines: lns

	| texts |
	texts := OrderedCollection new.
	lns do: [ :l | texts addAll: l texts ].
	^self htmlFromTexts: texts.
!
   
htmlFromTexts: texts

	| codeBlock str |
	codeBlock := false.
	texts isEmpty ifTrue: [ ^nil ].
	str := WriteStream on: String new.
	texts do: [ :t | 
		(#('7' '18') includes: t font) ifTrue: [
			t == texts first ifFalse: [ str space ].
			codeBlock ifFalse: [ str nextPutAll: '<code>' ].
			codeBlock := true.
		] ifFalse: [ (#('1' '4') includes: t font) ifTrue: [
			codeBlock ifTrue: [ str nextPutAll: '</code>' ].
			codeBlock := false.
			t == texts first ifFalse: [ str space ].
		] ifFalse: [
			self error: 'Unknown font'.
		] ].

		t isItalic ifTrue: [ str nextPutAll: '<em>' ].
		str nextPutAll: t text.
		t isItalic ifTrue: [ str nextPutAll: '</em>' ].
	].
	^str contents.!
 
new
    
    
    ^super new initialize; yourself.!
  
readSection: line ofSize: sectionSize ensureType: typeString
    
    | str section parts |
        str := (line replaceAllChar: Tab withChar: Space) trimBlanks asStream.
    str atEnd ifTrue: [ ^nil ].
    section := (str upTo: Space) trimBlanks.
    parts := section asArrayOfSubstringsSeparatedBy: $..
    parts size = sectionSize ifFalse: [ ^nil ].
    parts do: [ :part |
        part trimBlanks isEmpty ifTrue: [ ^nil ].
        part do: [ :chr | chr isDigit ifFalse: [ ^nil ] ].
    ].
    str skipSeparators.
    str atEnd ifTrue: [ ^nil ].
    (str peekRest indexOfString: typeString) = 1 ifFalse: [ ^nil ].
    str skip: typeString size.
    str skipSeparators.
    str rest trimBlanks isEmpty ifTrue: [ ^nil ].
    ^section.!
  
readSectionName: line sectionSize: sectionSize ensureType: typeString
    
    | str section parts |
        str := (line replaceAllChar: Tab withChar: Space) trimBlanks asStream.
    str atEnd ifTrue: [ ^nil ].
    section := (str upTo: Space) trimBlanks.
    parts := section asArrayOfSubstringsSeparatedBy: $..
    parts size = sectionSize ifFalse: [ ^nil ].
    parts do: [ :part |
        part trimBlanks isEmpty ifTrue: [ ^nil ].
        part do: [ :chr | chr isDigit ifFalse: [ ^nil ] ].
    ].
    str skipSeparators.
    str atEnd ifTrue: [ ^nil ].
    (str peekRest indexOfString: typeString) = 1 ifFalse: [ ^nil ].
    str skip: typeString size.
    str skipSeparators.
    ^str rest trimBlanks.!

removeTags: aString
    
    | in out |
        in := aString asStream.
    out := WriteStream on: (String new: aString size).
    [ in atEnd ] whileFalse: [
        out nextPutAll: (in upTo: $<).
        in atEnd ifFalse: [ in upTo: $> ].
    ].
    ^out contents.!
 
replaceHtml: aString
    
    
    ^((aString replaceAll: '&gt;' with: '>') replaceAll: '&lt;' with: '<') replaceAll: '&amp;' with: '&'.!

toText: aCollection
     
    | tmp |
	
	tmp := WriteStream on: String new.
	aCollection do: [ :e | tmp nextPutAll: (self replaceHtml: (self removeTags: e asString)) trimBlanks ] andBetweenDo: [ tmp cr ].
	^tmp contents.! !

!STMessage methodsFor: 'as yet unclassified' !
 
definition

	^definition.!

docSection
     
    	^docSection.!
   
errors

	^errors.!

initialize
    
        super initialize.
    parameters := OrderedCollection new.
    refinements := OrderedCollection new.    
    names := OrderedCollection new.
    selectors := OrderedCollection new.
    synopsisLines := OrderedCollection new.
    definitionLines := OrderedCollection new.
    errors := OrderedCollection new.
!
 
parameters

	^parameters.!

printOn: str
	
    
	str nextPutAll: docSection asString.
	isRefined == true ifTrue: [ str nextPutAll: ' Message Refinement: ' ] ifFalse: [ str nextPutAll: ' Message: ' ].
	str nextPutAll: (names at: 1 ifAbsent: [ nil ]) asString.
	str nextPutAll: ' (#'.
	str nextPutAll: (selectors at: 1 ifAbsent: [ nil ]) asString.
	str nextPutAll: ')'.
!
  
processBodyLines: idx

	| lastSectionIndex returnLines refs currentRefLines n defProtocol |
	lastSectionIndex := nil.
	refs := OrderedCollection new.
	returnLines := OrderedCollection new.
	idx to: lines size do: [ :i |
		| line str sectionIdx |
		line := lines at: i.
		str := line asString.
		sectionIdx := self sectionIndex: line.
		(sectionIdx = lastSectionIndex) & (sectionIdx ~= 3) ifTrue: [ self error: 'Section being repeated' ].
		sectionIdx notNil & (lastSectionIndex ~= 99) ifTrue: [ lastSectionIndex := sectionIdx ].
		(lastSectionIndex = 1) & sectionIdx isNil ifTrue: [ synopsisLines add: line ].
		(lastSectionIndex = 2) ifTrue: [
			sectionIdx notNil ifTrue: [ 
				defProtocol := ((self replaceHtml: (self removeTags: line asString)) trimBlanks asStream upTo: Space; rest).
			] ifFalse: [ 
				definitionLines add: line 
			].
		].
		(lastSectionIndex = 3) ifTrue: [
			sectionIdx notNil ifTrue: [
				currentRefLines := OrderedCollection new.
				refs add: ((self replaceHtml: (self removeTags: line asString)) trimBlanks asStream upTo: Space; rest) -> currentRefLines.
			] ifFalse: [
				currentRefLines add: line.
			].
		].
		(lastSectionIndex = 4) & sectionIdx isNil ifTrue: [ parameters add: (STParameter fromLine: line) ].
		(lastSectionIndex = 5) & sectionIdx isNil ifTrue: [ returnLines add: line ].
		(lastSectionIndex = 6) & sectionIdx isNil ifTrue: [ errors add: line ].
	].

	synopsis := self htmlFromLines: synopsisLines.
	definition := self htmlDescriptionFromLines: definitionLines.
	definition isNil ifFalse: [ 
		(defProtocol first = $<) & (defProtocol last = $>) ifFalse: [ self error: 'Protocol name' ].
		defProtocol := defProtocol copyFrom: 2 to: defProtocol size - 1.
		defProtocol = 'dyadic-valuable' ifTrue: [ defProtocol := 'dyadicValuable' ].
		defProtocol = 'sequenceReadableCollection' ifTrue: [ defProtocol := 'sequencedReadableCollection' ].
		defProtocol = 'sequenceContractibleCollection' ifTrue: [ defProtocol := 'sequencedContractibleCollection' ].
		docSection = '5.8.2.12' ifTrue: [ defProtocol := 'Duration' ].
		definition := defProtocol -> definition 
	].
	returnValue := STReturnValue new.
	returnValue lines addAll: returnLines.
	(errors size = 1 and: [ errors first asString trimBlanks asLowercase = 'none' ])
		ifTrue: [ errors removeAll ].
	docSection = '5.7.11.2' ifTrue: [ errors removeAll ].
	errors := self htmlDescriptionFromLines: errors.

	selectors size = 0 ifTrue: [ self error: 'No selectors' ].
	(selectors size = 1 and: [ (Message numberOfArgumentsFor: selectors first) ~= parameters size ]) ifTrue: [ 
		parameters size = 0 ifFalse: [ Transcript show: ('WARNING: Param mismatch'->self->parameters size) asString; cr ].
	].

	refs do: [ :assoc |
		n := assoc key.
		(n first = $<) & (n last = $>) ifFalse: [ self error: 'Protocol name' ].
		n := n copyFrom: 2 to: n size - 1.
		n = 'sequenceReadableCollection' ifTrue: [ n := 'sequencedReadableCollection' ].
		refinements add: n -> (self htmlDescriptionFromLines: assoc value).
	].!
 
processLines

	| idx tmp |
	
	docSection := self class readSection: (self removeTags: lines first asString) ofSize: 4 ensureType: 'Message:'.
	docSection isNil ifTrue: [
		docSection := self class readSection: (self removeTags: lines first asString) ofSize: 4 ensureType: 'Message Refinement:'.
		isRefined := true.
	] ifFalse: [
		isRefined := false.
	].
	docSection := self replaceHtml: docSection.
	tmp := (self class readSectionName: (self removeTags: lines first asString) sectionSize: 4 ensureType: 
		(isRefined ifTrue: [ 'Message Refinement:' ] ifFalse: [ 'Message:' ])).
	idx := #( '5.7.8.23' '5.7.10.12' '5.7.12.5' '5.7.17.7' '5.7.17.8' '5.8.4.2' '5.8.4.3' '5.8.4.5' ) indexOf: docSection.
	idx = 0 ifTrue: [
		self processName: tmp.
		idx := 2.
	] ifFalse: [
		self processName: tmp, 
			(#(' exceptionHandler' ' replacementElement' ' replacementStart' ' replacementElement' ' replacementElement' 
			' second: second' ' second: second offset: offset' ' offset: offset') at: idx).
		idx := 3.
	].

	[ idx <= lines size and: [ ( self sectionIndex: (lines at: idx) ) isNil ] ] whileTrue: [ 
		| str |
		str := self removeTags: (lines at: idx) asString trimBlanks.
		(str indexOfString: 'Message:') = 1 ifTrue: [ str := (str copyFrom: 9) trimBlanks ].
		(str indexOfString: 'Message Refinement:') = 1 ifTrue: [ str := (str copyFrom: 20) trimBlanks ].
		self processName: str.
		idx := idx + 1.
	].

	self processBodyLines: idx.

	parameters do: [ :p | p processLines ].
	returnValue isNil ifFalse: [ returnValue processLines ].!
   
processName: aName

	| aSelector str |
	
	str := self replaceHtml: aName.
	names add: str.
	aSelector := str trimBlanks replaceAllChar: Tab withChar: Space.
	(aSelector includes: $:) ifTrue: [ 
		aSelector := (aSelector asArrayOfSubstringsSeparatedBy: $:).
		aSelector := aSelector copyFrom: 1 to: aSelector size - 1.
		aSelector := aSelector inject: '' into: [ :s :e | s, (e reversed asStream upTo: Space) reversed, ':' ].
	] ifFalse: [
		(aSelector includes: Space) ifTrue: [ aSelector := aSelector asStream upTo: Space ].
	].
	selectors add: aSelector.!
  
refinements

	^refinements.!
  
returnValue

	^returnValue.!
  
sectionIndex: aXLine

	| str |
		str := aXLine asString trimBlanks.
	( #('<b>Synopsis</b>' '<b>Synopsis:</b>') includes: str) ifTrue: [ ^1 ].
	(str indexOfString: '<b>Definition: ') = 1 ifTrue: [ ^2 ].
	(str indexOfString: '<b>Refinement: ') = 1 ifTrue: [ ^3 ].
	( #('<b>Parameters</b>') includes: str) ifTrue: [ ^4 ].
	( #('<b>Return Value</b>' '<b>Return Values</b>' '<b>Return Value:</b>' '<b>Return Values:</b>') includes: str) ifTrue: [ ^5 ].
	( #('<b>Errors</b>') includes: str) ifTrue: [ ^6 ].

	(str asLowercase indexOfString: '<b>') = 1 ifTrue: [ 
		aXLine texts first left < 109 ifTrue: [ ^99 ].
	].
	^nil.




	!
  
selectors

	^selectors.!
  
synopsis
     
    
	^synopsis.!
 
writeXmlOn: str

	names with: selectors do: [ :n :s | self writeXmlOn: str name: n selector: s ].!

writeXmlOn: str name: name selector: selector

	str nextPutAll: '<Message'.
"	self writeXmlAttribute: 'name' value: name on: str. "
	self writeXmlAttribute: 'selector' value: selector on: str.
	self writeXmlAttribute: 'docId' value: docSection on: str.
	self writeXmlAttribute: 'refined' value: (isRefined ifTrue: [ 'true' ] ifFalse: [ 'false' ]) on: str.
	str nextPutAll: '>'; cr.
	str nextPutAll: '<Synopsis>'.
	synopsis isNil ifFalse: [ self writeXmlText: synopsis on: str ].
	str nextPutAll: '</Synopsis>'; cr.
	definition isNil ifFalse: [ 
		str nextPutAll: '<Definition'.
		self writeXmlAttribute: 'protocol' value: definition key on: str. 
		str nextPutAll: '>'.
		self writeXmlText: definition value on: str.
		str nextPutAll: '</Definition>'; cr.
	].

	refinements do: [ :assoc | 
		str nextPutAll: '<Refinement'. 
		self writeXmlAttribute: 'protocol' value: assoc key on: str. 
		str nextPutAll: '>'.
		self writeXmlText: assoc value on: str.
		str nextPutAll: '</Refinement>'; cr 
	].
	parameters do: [ :e | e writeXmlOn: str ].
	returnValue isNil ifFalse: [ returnValue writeXmlOn: str ].
	errors isNil ifFalse: [ str nextPutAll: '<Errors>'. self writeXmlText: errors on: str. str nextPutAll: '</Errors>'; cr. ].
	str nextPutAll: '</Message>'; cr.! !

!STMessage class methodsFor: 'as yet unclassified' !
 
isMessageStart: line
    "
        Example:
            5.3.1 Protocol: <Object>
    "
    ^(self readSection: line ofSize: 4 ensureType: 'Message:') notNil
        or: [ (self readSection: line ofSize: 4 ensureType: 'Message Refinement:') notNil ].


! !

!STParameter methodsFor: 'as yet unclassified' !

printOn: str
     
    	lines printOn: str
!
 
processLines

	| str prots p |
	lines size ~= 1 ifTrue: [ self error: 'Params problem' ].
	str := lines first asString trimBlanks replaceAllChar: Tab withChar: Space.
	str = 'UNSPECIFIED' ifTrue: [ ^Transcript show: 'WARNING: Param '; show: str; cr ].
	str := (self replaceHtml: (self removeTags: str)) asStream .
	name := str upTo: Space.	
	str skipSeparators.

	prots := OrderedCollection new.
	str peek = $< ifFalse: [ self error: 'Expected protocol' ].
	str skip: 1.
	prots add: (str upTo: $>) trimBlanks.
	str atEnd ifTrue: [ self error: 'Invalod protocol' ].
	str skipSeparators.
	[ str peek = $| ] whileTrue: [
		str skip: 1; skipSeparators.
		str peek = $< ifFalse: [ self error: 'Expected protocol' ].
		str skip: 1.
		prots add: (str upTo: $>) trimBlanks.
		str atEnd ifTrue: [ self error: 'Invalod protocol' ].
		str skipSeparators.
	].
	aliasing := str upTo: Space.
	str atEnd ifFalse: [ ^self error: 'Something wrotten' ].
	( #( 'unspecified' 'uncaptured' 'captured' ) includes: aliasing)
		ifFalse: [ self error: 'Unknown aliasing' ].
	prots isEmpty ifTrue: [ self error: 'No protocols' ].
	prots do: [ :pr |
		p := pr.
		p = 'sequenceReadableCollection' ifTrue: [ p := 'sequencedReadableCollection' ].
		p = 'character' ifTrue: [ p := 'Character' ].
		p = 'Integer' ifTrue: [ p := 'integer' ].
		protocols add: p.
	].!

writeXmlOn: str

	name isNil ifTrue: [ ^str nextPutAll: '<Parameter name="ERROR" aliasing="unspecified"><Protocol name="ANY"/></Parameter>' ].

	str nextPutAll: '<Parameter'.
	self writeXmlAttribute: 'name' value: name on: str. 
	self writeXmlAttribute: 'aliasing' value: aliasing on: str.
	str nextPutAll: '>'; cr.
	protocols do: [ :p | 
		str nextPutAll: '<Protocol'. 
		self writeXmlAttribute: 'name' value: p on: str. 
		str nextPutAll: '/>'; cr.
	].
	str nextPutAll: '</Parameter>'; cr.! !

!STProtocol methodsFor: 'as yet unclassified' !
  
conformsTo 

	^conformsTo .!
  
description
    
        ^description.!
   
docSection
     
    	^docSection.!
   
initialize
    
        super initialize.
    conformsTo := OrderedCollection new.
    messages := OrderedCollection new.
    descriptionLines := OrderedCollection new.
    standardGlobalLines := OrderedCollection new.
    messageLines := OrderedCollection new.
    standardGlobals := OrderedCollection new.
!
  
messages
     
    
	^messages.!
 
name

	^name.!

printOn: str
    
    
    str nextPutAll: docSection asString.
    str nextPutAll: ' Protocol: <'.
    str nextPutAll: name asString.
    str nextPutAll: '>'.!
  
processLines

	| tmp max line msg map |
	
	docSection := self replaceHtml: (self class readSection: (self removeTags: lines first asString) ofSize: 3 ensureType: 'Protocol:').
	name := self replaceHtml: (self class readSectionName: (self removeTags: lines first asString) sectionSize: 3 ensureType: 'Protocol:').
	(name first = $<) & (name last = $>) ifFalse: [ self error: 'Protocol name' ].
	name := name copyFrom: 2 to: name size - 1.
	isAbstract := name first isLowercase.

	((lines at: 2) asString trimBlanks = '<b>Conforms To</b>') | ((lines at: 2) asString trimBlanks = '<b>Conforms To:</b>') ifFalse: [ self error: 'No Conforms To ' ].
	tmp := self replaceHtml: (self removeTags: (lines at: 3) asString).
	tmp := tmp replaceAll: '> <' with: '>, <'.
	docSection = '5.5.17' ifTrue: [ tmp := '<exceptionSelector>' ].
	docSection = '5.7.22' ifTrue: [ tmp := '<collection factory>' ].

	conformsTo := ((tmp asArrayOfSubstringsSeparatedBy: $,) collect: [ :e | e trimBlanks ]) collect: [ :e |
		(e first = $<) & (e last = $>) ifFalse: [ self error: 'Protocol name' ].
		e copyFrom: 2 to: e size - 1.
	].
	conformsTo isEmpty ifTrue: [ self error: 'No Conforms To ' ].

	((lines at: 4) asString trimBlanks = '<b>Description</b>') |
		(((lines at: 4) asString trimBlanks indexOfString: '<b>Description: ') = 1) ifFalse: [ self error: 'No Description ' ].
	max := self addLinesFrom: lines to: descriptionLines startingAt: 5 endOnSections: #('<b>Messages</b>' '<b>Messages:</b>' '<b>Standard Globals</b>' ).
	description := self htmlDescriptionFromLines: descriptionLines.
	description isEmpty ifTrue: [ self error: 'No Description ' ].

	(lines at: max) asString trimBlanks = '<b>Standard Globals</b>' ifTrue: [
		max := self addLinesFrom: lines to: standardGlobalLines startingAt: max + 1 endOnSections: #('<b>Messages</b>' '<b>Messages:</b>' ).
	].

	( #('<b>Messages</b>' '<b>Messages:</b>') includes: (lines at: max) asString trimBlanks )
		ifFalse: [ self error: 'No messages' ].

	max + 1 to: lines size do: [ :idx |
		| x3j20line |
		x3j20line := lines at: idx.
		line := self removeTags: x3j20line asString.

		(STMessage isMessageStart: line) ifTrue: [
			msg isNil ifFalse: [ messages add: msg ].
			msg := STMessage new.
		].
		msg isNil
			ifTrue: [ messageLines add: x3j20line ]
			ifFalse: [ msg lines add: x3j20line ].
		].
	msg isNil ifFalse: [ messages add: msg ].

	messages do: [ :e | e processLines ].
	(messageLines size = 1 and: [ messageLines first asString trimBlanks asLowercase = 'none' ]) 
		ifTrue: [ messageLines removeAll ].

	map := Dictionary new.   
	messages do: [ :m |
		m docSection = '5.5.18.3' ifFalse: [
			m selectors do: [ :s |
				(map includesKey: s) ifTrue: [ self error: 'Selector already defined.' ].
				map at: s put: m.
			].
		].
	].

	messageLines do: [ :e |  
		| s |
		s := (self replaceHtml: e asString) trimBlanks.
		(self docSection = '5.6.2') & (s = '- operand') ifTrue: [ s:= '-' ].
		(self docSection = '5.7.23') & (s = 'with:with:with') ifTrue: [ s:= 'with:with:with:' ].
		(self docSection = '5.7.24') & (s = 'with:with:with') ifTrue: [ s:= 'with:with:with:' ].
		(self docSection = '5.7.25') & (s = 'with:with:with') ifTrue: [ s:= 'with:with:with:' ].
		(self docSection = '5.7.26') & (s = 'with:with:with') ifTrue: [ s:= 'with:with:with:' ].
		(self docSection = '5.7.27') & (s = 'with:with:with') ifTrue: [ s:= 'with:with:with:' ].
		(self docSection = '5.7.28') & (s = 'with:with:with') ifTrue: [ s:= 'with:with:with:' ].
		(self docSection = '5.7.29') & (s = 'with:with:with') ifTrue: [ s:= 'with:with:with:' ].
		(self docSection = '5.7.30') & (s = 'with:with:with') ifTrue: [ s:= 'with:with:with:' ].
		(map includesKey: s) ifFalse: [ self error: 'Message not defined' ].
	].

	self processStandardGlobalLines.!
 
processStandardGlobalLines


	| gl |
	standardGlobalLines do: [ :line |
		(docSection = '5.3.10') | (docSection = '5.10.4') ifTrue: [
			(line == standardGlobalLines first) ifTrue: [
				gl := STStandardGlobal new. 
				gl name: line texts first text trimBlanks.
				standardGlobals add: gl.
			].
		] ifFalse: [
			line texts first left < 180 ifTrue: [ 
				gl := STStandardGlobal new. 
				gl name: line texts first text trimBlanks.
				standardGlobals add: gl.
			].
		].
		gl lines add: line.
	].
	standardGlobals do: [ :e | e processLines ].!
 
standardGlobalLines

	^standardGlobalLines.!
  
standardGlobals

	^standardGlobals.!
  
writeXmlOn: str 

	str nextPutAll: '<Protocol'.
	self writeXmlAttribute: 'name' value: name on: str.
	self writeXmlAttribute: 'docId' value: docSection on: str.
	self writeXmlAttribute: 'abstract' value: (isAbstract ifTrue: [ 'true' ] ifFalse: [ 'false' ]) on: str.
	str nextPutAll: '>'; cr.
	self conformsTo do: [ :e | str nextPutAll: '<ConformsTo'. self writeXmlAttribute: 'protocol' value: e on: str. str nextPutAll: '/>'; cr ].
	str nextPutAll: '<Description>'.
	self writeXmlText: description on: str.
	str nextPutAll: '</Description>'; cr.
	self standardGlobals do: [ :e | e writeXmlOn: str ].
	self messages do: [ :e | e writeXmlOn: str ].
	str nextPutAll: '</Protocol>'; cr.! !

!STProtocol class methodsFor: 'as yet unclassified' !
 
isProtocolStart: line
    "
        Example:
            5.3.1 Protocol: <Object>
    "
    ^(self readSection: line ofSize: 3 ensureType: 'Protocol:') notNil.! !

!STReturnValue methodsFor: 'as yet unclassified' !
   
processHeaderLine

	| str prots p |
	lines size = 0 ifTrue: [ ^nil ].
	str := lines first asString trimBlanks replaceAllChar: Tab withChar: Space.
	str = 'UNSPECIFIED' ifTrue: [ ^self ].
	str = 'none' ifTrue: [ ^self ].
	str = 'The Default Result Type' ifTrue: [ ^nil ].
	str = 'If the operands conform to integer then &lt;rational&gt;' ifTrue: [ ^nil ].
	str = '<b>receiver result</b>' ifTrue: [ ^nil ].
	str = 'SortedCollection new' ifTrue: [ str := '&lt;SortedCollection&gt; new' ].
	str := (self replaceHtml: (self removeTags: str)) trimBlanks asStream .
	prots := OrderedCollection new.
	str peek = $< ifFalse: [ self error: 'Expected protocol' ].
	str skip: 1.
	prots add: (str upTo: $>) trimBlanks.
	str atEnd ifTrue: [ self error: 'Invalod protocol' ].
	str skipSeparators.
	[ str peek = $| ] whileTrue: [
		str skip: 1; skipSeparators.
		str peek = $< ifFalse: [ self error: 'Expected protocol' ].
		str skip: 1.
		prots add: (str upTo: $>) trimBlanks.
		str atEnd ifTrue: [ self error: 'Invalod protocol' ].
		str skipSeparators.
	].
	aliasing := str upTo: Space.
	str atEnd ifFalse: [ ^self error: 'Something wrotten' ].
	prots isEmpty ifTrue: [ self error: 'No protocols' ].
	prots do: [ :pr |
		p := pr.
		p = 'sequenceReadableCollection' ifTrue: [ p := 'sequencedReadableCollection' ].
		p = 'character' ifTrue: [ p := 'Character' ].
		p = 'Integer' ifTrue: [ p := 'integer' ].
		protocols add: p.
	].
	aliasing = 'receiver' ifTrue: [ aliasing := 'state'. ^nil ].
	aliasing = 'parameter' ifTrue: [ aliasing := 'state'. ^nil ].
	( #( 'unspecified' 'state' 'new' ) includes: aliasing)
		ifFalse: [ self error: 'Unknown aliasing' ].!
 
processLines

	| idx |
	self processHeaderLine isNil ifTrue: [ idx := 1 ] ifFalse: [ idx := 2 ].
	description := self htmlDescriptionFromLines: (lines copyFrom: idx).!
 
writeXmlOn: str

	description isNil & protocols isEmpty & aliasing isNil ifTrue: [ ^self ].

	str nextPutAll: '<ReturnValue'.
	self writeXmlAttribute: 'aliasing' value: (aliasing ifNil: [ 'unspecified' ]) on: str.
	str nextPutAll: '>'; cr.
	protocols do: [ :p | 
		str nextPutAll: '<Protocol'. 
		self writeXmlAttribute: 'name' value: p on: str. 
		str nextPutAll: '/>'; cr.
	].
	description isNil ifFalse: [
		str nextPutAll: '<Description>'.
		self writeXmlText: description on: str.
		str nextPutAll: '</Description>'; cr.
	].
	str nextPutAll: '</ReturnValue>'; cr.! !

!STSpecification methodsFor: 'as yet unclassified' !

initialize
    
        super initialize.
    protocols := OrderedCollection new.!
   
protocols

	^protocols.! !

!STSpecification class methodsFor: 'as yet unclassified' !
  
fromLine: line
     
    | tmp |
	
	tmp := self new.
	tmp lines add: line.
	^tmp.! !

!STStandardGlobal methodsFor: 'as yet unclassified' !
 
name

	^name.!

name: aString

	name := aString.!
 
printOn: str
	str nextPutAll: name asString.!
  
processLines

	| texts |
	texts := OrderedCollection new.
	texts addAll: (lines first texts copyFrom: 2).
	2 to: lines size do: [ :i | texts addAll: (lines at: i) texts ].

	description := self htmlFromTexts: texts.!
 
writeXmlOn: str

	str nextPutAll: '<StandardGlobal'.
	self writeXmlAttribute: 'name' value: name on: str.
	str nextPutAll: '>'.
	self writeXmlText: description on: str.
	str nextPutAll: '</StandardGlobal>'; cr.! !

!XDoc methodsFor: 'as yet unclassified' !
  
allLinesDo: aBlock
    
        self pages do: [ :page |
        page lines do: [ :line | aBlock value: line ].
    ].!
 
cleanUp
     
    

    self pages do: [ :page |
        page texts removeAll: (page texts select: [ :e | e text = 'Licensed to Todor Todorov. ANSI order X_175234. Downloaded 6/9/2010 9:13 PM. Single user license only. Copying and networking prohibited.' ]).
        page texts removeAll: (page texts select: [ :e | e text = 'ANSI NCITS 319-1998' ]).
        page texts first text = (page number - 8) asString ifTrue: [ page texts removeFirst ].
    ].!
   
fromXML: anSSDXMLElement
     
    anSSDXMLElement nodeName = 'pdf2xml' ifFalse: [ ^self error: 'Wrong node' ].

    anSSDXMLElement childNodes do: [ :xmlNode |
        xmlNode nodeName = 'page'
            ifTrue: [ pages add: (XPage fromXML: xmlNode document: self) ]
            ifFalse: [ self error: 'Wrong node' ].
    ].!

getFont: fontId

	self pages do: [ :page |
		page fonts do: [ :font |
			font id = fontId ifTrue: [ ^font ].
		].
	].
	^nil.!

initialize
     
        pages := OrderedCollection new.
    super initialize.!
  
pages
     
    
    ^pages.! !

!XDoc class methodsFor: 'as yet unclassified' !
   
fromFile: filename
     
    
    ^self fromXML: (SSDXMLParser load: filename).! !

!XFont methodsFor: 'as yet unclassified' !
 
isItalic

	^#('4' '18') includes: self id.! !

!XFont methodsFor: 'accessing' !
 
color
     
    
    ^color.!

family
     
    
    ^family.!
  
fontSize
     
    
    ^size.!
  
id
     
    
    ^id.!
  
page
     
    
    ^page.
! !

!XFont methodsFor: 'private' !

fromXML: anSSDXMLElement
     
    anSSDXMLElement nodeName = 'fontspec' ifFalse: [ self error: 'Wrong node' ].

    anSSDXMLElement attributes do: [ :xmlAttr | self readXmlAttribute: xmlAttr ].

    anSSDXMLElement childNodes do: [ :xmlElem | self error: 'Wrong node' ].!
  
initialize
     
    
    super initialize.!
 
page: xPage
     
    
    page := xPage.!
   
readXmlAttribute: anSSDXMLAttribute
     

    anSSDXMLAttribute nodeName = 'id' ifTrue: [ ^id := anSSDXMLAttribute nodeValue ].
    anSSDXMLAttribute nodeName = 'size' ifTrue: [ ^size := anSSDXMLAttribute nodeValue asInteger ].
    anSSDXMLAttribute nodeName = 'family' ifTrue: [ ^family := anSSDXMLAttribute nodeValue ].
    anSSDXMLAttribute nodeName = 'color' ifTrue: [ ^color := anSSDXMLAttribute nodeValue ].

    self error: 'Wrong attribute'.! !

!XFont class methodsFor: 'as yet unclassified' !
   
fromXML: anSSDXMLElement page: page
     
    
    ^(super fromXML: anSSDXMLElement) page: page; yourself.! !

!XItem class methodsFor: 'as yet unclassified' !

fromXML: anSSDXMLElement
     
    
    ^self new fromXML: anSSDXMLElement; yourself.!
   
new
     
    
    ^super new initialize; yourself.! !

!XLine methodsFor: 'accessing' !
   
document

	^self page document.!
  
page
     
    
    ^page.
!

texts
     
    
    ^texts.!

top
     
    
    ^top.
    ! !

!XLine methodsFor: 'as yet unclassified' !
  
asString
    
    | str |
        str := WriteStream on: (String new: 100).
    self printOn: str.
    ^str contents replaceAll: '</b> <b>' with: ' '.! !

!XLine methodsFor: 'private' !

initialize
     
    
    texts := SortedCollection sortBlock: [ :a :b | a left < b left ].
    super initialize.!
  
page: xPage
     
    
    page := xPage.
!
 
printOn: str
     
    
    self texts do: [ :txt | txt printOn: str. str space. ].
    str cr.!

top: int
     
    
    top := int.
! !

!XLine class methodsFor: 'as yet unclassified' !
 
top: top page: page
     
    
    ^self new top: top; page: page; yourself.! !

!XPage methodsFor: 'accessing' !
  
document

    ^document.!
 
fonts


    ^fonts.!
 
lines
     
    
    lines isNil ifTrue: [ lines := self getLines ].
    ^lines.!
   
number
     
    
    ^number.!
  
texts
     
    
    ^texts.! !

!XPage methodsFor: 'private' !

document: xDocument

    document := xDocument!
   
fromXML: anSSDXMLElement
     
    anSSDXMLElement nodeName = 'page' ifFalse: [ self error: 'Wrong node' ].

    anSSDXMLElement attributes do: [ :xmlAttr | self readXmlAttribute: xmlAttr ].

    anSSDXMLElement childNodes do: [ :xmlElem |
        xmlElem nodeName = 'fontspec' ifTrue: [ fonts add: (XFont fromXML: xmlElem page: self) ] ifFalse: [
        xmlElem nodeName = 'text' ifTrue: [ texts add: (XText fromXML: xmlElem page: self) ]
        ifFalse: [ self error: 'Wrong node' ] ].
    ].!
 
getLines
     

    | result txts txt line |
    result := OrderedCollection new.
    txts := self texts copy.

    [ txts isEmpty ] whileFalse: [
        txt := txts at: 1.
        line := XLine top: txt top page: self.
        line texts addAll: (txts select: [ :t | t top between: txt top - 2 and: txt top + 2 ]).
        txts removeAll: line texts.
        result add: line.
    ].

    ^result.!
 
initialize
     
    
    fonts := OrderedCollection new.
    texts := OrderedCollection new.
    super initialize.!
   
printOn: str
     
    
    str nextPutAll: 'Page: '; nextPutAll: number asString; cr.
    self lines do: [ :line | line printOn: str ].
!
 
readXmlAttribute: anSSDXMLAttribute
     

    (anSSDXMLAttribute nodeName = 'position' and: [ anSSDXMLAttribute nodeValue = 'absolute' ] ) ifTrue: [ ^self ].
    (anSSDXMLAttribute nodeName = 'top' and: [ anSSDXMLAttribute nodeValue = '0' ] ) ifTrue: [ ^self ].
    (anSSDXMLAttribute nodeName = 'left' and: [ anSSDXMLAttribute nodeValue = '0' ] ) ifTrue: [ ^self ].
    (anSSDXMLAttribute nodeName = 'height' and: [ anSSDXMLAttribute nodeValue = '1188' ] ) ifTrue: [ ^self ].
    (anSSDXMLAttribute nodeName = 'width' and: [ anSSDXMLAttribute nodeValue = '918' ] ) ifTrue: [ ^self ].
    anSSDXMLAttribute nodeName = 'number' ifTrue: [ ^number := anSSDXMLAttribute nodeValue asInteger ].

    self error: 'Wrong attribute'.! !

!XPage class methodsFor: 'as yet unclassified' !
  
fromXML: anSSDXMLElement document: document
     
    
    ^(super fromXML: anSSDXMLElement) document: document; yourself.! !

!XText methodsFor: 'accessing' !

document

	^self page document.!
  
font

    ^font.!
 
getFont

	^self document getFont: self font.!
 
height

    ^height.!
 
left


    ^left.!
   
page


    ^page.!
   
text

    ^text.!
 
top

    ^top.!
   
width

    ^width.! !

!XText methodsFor: 'as yet unclassified' !
   
isItalic

	^self getFont isItalic.! !

!XText methodsFor: 'private' !
   
emitTextNode: xmlNode on: str

    xmlNode isXMLTextNode ifTrue: [
        str nextPutAll: xmlNode nodeValue asHTMLText.
    ] ifFalse: [
        (xmlNode attributes length ~= 0)
            ifTrue: [ ^self error: 'Wrong node' ].
        str nextPut: $<; nextPutAll: xmlNode nodeName; nextPut: $>.
        xmlNode childNodes do: [ :e | self emitTextNode: e on: str ].
        str nextPut: $<; nextPut: $/; nextPutAll: xmlNode nodeName; nextPut: $>.
    ].!
  
fromXML: anSSDXMLElement

    | str |
    anSSDXMLElement nodeName = 'text' ifFalse: [ self error: 'Wrong node' ].

    anSSDXMLElement attributes do: [ :xmlAttr | self readXmlAttribute: xmlAttr ].

    str := WriteStream on: String new.
    anSSDXMLElement childNodes do: [ :xmlNode | self emitTextNode: xmlNode on: str ].
    text := str contents.!
 
initialize

    super initialize.!

page: xPage


    page := xPage.!

printOn: str


    str nextPutAll: text asString.!
   
readXmlAttribute: anSSDXMLAttribute

    anSSDXMLAttribute nodeName = 'top' ifTrue: [ ^top := anSSDXMLAttribute nodeValue asInteger ].
    anSSDXMLAttribute nodeName = 'left' ifTrue: [ ^left := anSSDXMLAttribute nodeValue asInteger ].
    anSSDXMLAttribute nodeName = 'width' ifTrue: [ ^width := anSSDXMLAttribute nodeValue asInteger ].
    anSSDXMLAttribute nodeName = 'height' ifTrue: [ ^height := anSSDXMLAttribute nodeValue asInteger ].
    anSSDXMLAttribute nodeName = 'font' ifTrue: [ ^font := anSSDXMLAttribute nodeValue ].

    self error: 'Wrong attribute'.! !

!XText class methodsFor: 'as yet unclassified' !
   
fromXML: anSSDXMLElement page: page
     
    
    ^(super fromXML: anSSDXMLElement) page: page; yourself.! !