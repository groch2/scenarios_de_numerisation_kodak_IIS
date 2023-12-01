' GECO1.XML - V2.01 - 2016-11-16 - AZ
' Mutuelle des Architectes Francais Assurance / Service des contrats
' Scénario de numérisation des Contrat GECO
' Ce scénario compte 3 formaulaires et déduit l'indexation d'un code à barre.
' Base : V1.0 (Basé sur CTPJ1.XML)
' Auteur : Therrien Wilfried

'MODIFICATIONS
' V2.01 - 2016-11-16 - AZ
' 	MAJ = Doc("DATE_DOCUMENT") = Doc("DateTraitement")
'
' V2
'    Ajout info nom adherent dans menu 5
'	 L'info n'est pas reconduite dans archeamaf
'	 '##WTH;20151015;V2
'
' V1.1
'    Ajout info nom adherent dans menu 5
'	 L'info n'est pas reconduite dans archeamaf
'
'-----------------------------------------------------------------------------------------
'-----------------------------------------------------------------------------------------
' CHANGELOG
'-----------------------------------------------------------------------------------------

Option Explicit

Dim MsgErreur
Dim G_suppverso, G_priorite
Dim G_msg
Dim MaxTop
Dim G_ForceTrans
Dim G_ActiveBenef

Dim adherentFounded ' True ou false pour dire si l'adhérent a été trouvé en BDD
Dim partieGaucheWex ' On met en variable globale cette derniére

MaxTop = 100
G_ForceTrans = 0
G_ActiveBenef = clng(0)

'=========================================================================================================
' 1 - Fonctions des requetes ValidationDef
'=========================================================================================================

'#################################################
Function RequeteListeFAMILLE()
	call TraceDebug("fn","RequeteListeFAMILLE","") 
	RequeteListeFAMILLE = "select distinct(FAMILLE) from C_CHARTEDOC where FILTRE = '1' AND ACTIVITE LIKE '%GECO1%' order by FAMILLE"
End function

'#################################################

Function RequeteListeCODE_GESTIONNAIRE_REDAC()
	call TraceDebug("fn","RequeteListeCODE_GESTIONNAIRE_REDAC","") 
	RequeteListeCODE_GESTIONNAIRE_REDAC = "select code_redacteur, Gestionnaire_Redacteur from V_GESTREDAC_PROD order by code_redacteur"
End function

'#################################################
Function RequeteListeTYPE_DOC()
	call TraceDebug("fn","RequeteListeTYPE_DOC","") 
	
	Dim IdEspace
	
	IdEspace = 	getEspaceID("CONTRAT", Doc("NO_CONTRAT"))
	
	if IdEspace = 3 then 
		RequeteListeTYPE_DOC = "select distinct(TYPE_DOC) from C_CHARTEDOC where FILTRE = '1' AND ACTIVITE LIKE '%EMOA1%' order by TYPE_DOC"
	else
		RequeteListeTYPE_DOC = "select distinct(TYPE_DOC) from C_CHARTEDOC where FILTRE = '1' AND ACTIVITE LIKE '%GECO1%' order by TYPE_DOC"
	end if
End function

'=========================================================================================================
' 2 - Fonctions de validation des champs - appelées par ATOME
'=========================================================================================================

'-------------------------------
' 2.1 - Index de niveau PLI
'-------------------------------
'#################################################
Function OPERATEUR_Validation()
	call TraceDebug("fn","OPERATEUR_Validation","") 
	OPERATEUR_Validation = True
	' ReportIndex : Transfert auto dans chaque doc dans DocExit()
End function

'#################################################

'CODE_GESTIONNAIRE_REDAC_Validation
Function CODE_GESTIONNAIRE_REDAC_Validation()
	call TraceDebug("fn","CODE_GESTIONNAIRE_REDAC_Validation","") 

	CODE_GESTIONNAIRE_REDAC_Validation = false

	' Forced
	if Doc("CODE_GESTIONNAIRE_REDAC").Forced = true then
	
		' Si le code gestionnaire redac est forcé et vide, on vide les champs associés
		if(Doc("CODE_GESTIONNAIRE_REDAC") = "")then
			CODE_GESTIONNAIRE_REDAC_Validation = False
			ReportForcedIndexTrans("CODE_GESTIONNAIRE_REDAC")
			exit function
		end if
		
		'Si on a une valeur de gestionnaire rédacteur
		if( len(Doc("CODE_GESTIONNAIRE_REDAC")) &gt; 0 )then
		
			'Si le plugin a rapatrié les informations du gestionnaire
			if (ListCODE_GESTIONNAIRE_REDAC_Plugin())  then
				ReportForcedIndexTrans("CODE_GESTIONNAIRE_REDAC")
				CODE_GESTIONNAIRE_REDAC_Validation = False
				exit function
			else
				alertInVcd("Code '"&amp; Doc("CODE_GESTIONNAIRE_REDAC") &amp;"' gestionnaire introuvable")
				Doc("CODE_GESTIONNAIRE_REDAC").Forced = false ' On déforce le champs
				' On vide les champs associés au code gestionnaire rédacteur
				CODE_GESTIONNAIRE_REDAC_Validation = False
				exit function
			end if
		else
			CODE_GESTIONNAIRE_REDAC_Validation = false
			exit function
		end if
	end if

	' Dans le cas ou il n'est pas forcé
	if( len(Doc("CODE_GESTIONNAIRE_REDAC")) &gt; 0) then
		' Si les informations du gestionnaire ont été trouvées
		'CODE_GESTIONNAIRE_REDAC_Validation = True
		
		'Si le plugin a rapatrié les informations du gestionnaire
		if (ListCODE_GESTIONNAIRE_REDAC_Plugin())  then
				'AZ 20160907
				ReportIndexTrans("CODE_GESTIONNAIRE_REDAC")
				ReportIndexDocRD("CODE_GESTIONNAIRE_REDAC")
				CODE_GESTIONNAIRE_REDAC_Validation = True
				exit function
		else
				alertInVcd("Code '"&amp; Doc("CODE_GESTIONNAIRE_REDAC") &amp;"' gestionnaire introuvable")
				' On vide les champs associés au code gestionnaire rédacteur
				CODE_GESTIONNAIRE_REDAC_Validation = False
				exit function
		end if
		
	else
		CODE_GESTIONNAIRE_REDAC_Validation = false
	end if
	
	if CODE_GESTIONNAIRE_REDAC_Validation then 
		'AZ 20160907 
		'ReportIndexTrans("CODE_GESTIONNAIRE_REDAC")
		ReportIndexDocRD("CODE_GESTIONNAIRE_REDAC")
		ReportIndexTrans("CODE_GESTIONNAIRE_REDAC")
	end if
		
	
End function

'#################################################
' NO_ADHERENT
Function NO_ADHERENT_Validation()
	call TraceDebug("fn","NO_ADHERENT_Validation","") 

	' Forced
	if (Doc("NO_ADHERENT").Forced = true) then
		'Si forcé avec une valeur
		if( len(Doc("NO_ADHERENT")) = 0) then
			' On vide la provenance correspondant (on garde le service)
			Doc("PROVENANCE") = "" 
		end if
		
		ReportForcedIndexTrans("NO_ADHERENT")
		
		' On précise que le N° d'adhérent n'est pas validé (forcé)
		NO_ADHERENT_Validation = False
		checkStatus()
		exit function
	end if
		
	' Si le numéro adhérent n'est pas forcé (implicite, voir ci dessus) et n'est pas vide
	if( len(Doc("NO_ADHERENT")) &gt; 0 ) then
		
		'V1.1' On ajoute la recherche du nom de l'adherent ici 
		getDataByAdherentV2(Doc("NO_ADHERENT"))
		' Si la fonction a trouvé l'adhérent
		NO_ADHERENT_Validation = True
	else
		NO_ADHERENT_Validation = false
	end if
	
	'On vérifie le statut du document en fonction de ce que l'on a renseigné
	checkStatus()
	
	' Le report est fait dans "ControlApresLecture"
	if NO_ADHERENT_Validation then ReportIndexDocRD("NO_ADHERENT")
	
	call CODE_BARRE_Validation()
	
End function

'#################################################
' NO_CONTRAT
Function NO_CONTRAT_Validation()
	call TraceDebug("fn","NO_CONTRAT_Validation","") 
	
	' Forced
	if (Doc("NO_CONTRAT").Forced = true) then
		ReportForcedIndexDocRD("NO_CONTRAT")
		NO_CONTRAT_Validation = False
		checkStatus() ' Vérification du statut du document
		MsgErreur = ""
		exit function
	end if
	
	if( G("Phase") = "VCD") then
		'et que le code à barre est vide
		if( len(Doc("CODE_BARRE")) &gt; 0 ) then
			' Si la récup retourne "true" c'est que l'on a trouvé la ligne
			if(getIndexByCodeBarre() = true) then				
				MsgErreur = ""
				CODE_BARRE_Validation = true
			else
				MsgErreur = "Valeur code à barre non référencée."
				CODE_BARRE_Validation = false
			end if
		end if

	end if

	 ' report
	if CODE_BARRE_Validation then 
		ReportIndexDocRD("CODE_BARRE")
		ReportIndexDocRD("NO_ADHERENT")	
		ReportIndexDocRD("NO_CONTRAT")		
		ReportIndexDocRD("DATE_DOCUMENT")	
		ReportIndexDocRD("NO_PROPOS")		 
		ReportIndexDocRD("LIB_DOC")		
		ReportIndexDocRD("NO_AVENANT")		
		ReportIndexDocRD("MultiCompteId")	
	end if	

	if ( G("Phase") = "LAD" and checkContratInBdd() = false) then
		MsgErreur =  "CE CONTRAT EST ABSENT DE LA BASE"
		NO_CONTRAT_Validation = false
	else
		if ListNO_CONTRATPlugin() then
			MsgErreur = ""
			NO_CONTRAT_Validation = true
		else
			MsgErreur = G_msg
			G_msg = ""
			NO_CONTRAT_Validation = false
		end if
	end if	
		
	checkStatus()  'Vérification du statut du document

	if NO_CONTRAT_Validation then 
		ReportIndexDocRD("NO_CONTRAT")
	end if
End function

'#################################################
' NO_AVENANT
Function NO_AVENANT_Validation()
	call TraceDebug("fn","NO_AVENANT_Validation","") 
	
	' Forced
	if (Doc("NO_AVENANT").Forced = true) then
		ReportForcedIndexDocRD("NO_AVENANT")
		NO_AVENANT_Validation = False
		checkStatus() ' Vérification du statut du document
		MsgErreur = ""
		exit function
	end if


	MsgErreur = ""
	
	NO_AVENANT_Validation = true
		
	checkStatus()  'Vérification du statut du document

	if NO_AVENANT_Validation then 
		ReportIndexDocRD("NO_AVENANT")
	end if
End function


'#################################################
' PROVENANCE
Function PROVENANCE_Validation()
	call TraceDebug("fn","PROVENANCE_Validation","") 
	PROVENANCE_Validation = True

'	' ReportIndex
	if PROVENANCE_Validation then ReportIndexDocRD("PROVENANCE")
	
End function

'#################################################
' Fonction qui valide la date de numérisation
Function DATE_NUM_Validation()
	call TraceDebug("fn","DATE_NUM_Validation","") 
	
	DATE_NUM_Validation = True
	
	if DATE_NUM_Validation then reportIndexDocRD("DATE_NUM")
	
end function

'#################################################
'-------------------------------
' 2.2 - Index de niveau DOCUMENT
'-------------------------------
'#################################################
' DATE_DOCUMENT
Function DATE_DOCUMENT_Validation()
	call TraceDebug("fn","DATE_DOCUMENT_Validation","") 

	DATE_DOCUMENT_Validation = true
	
	checkStatus()
	
	if DATE_DOCUMENT_Validation then ReportIndexDocRD("DATE_DOCUMENT")
End function

'#################################################
' FAMILLE
Function FAMILLE_Validation()
	call TraceDebug("fn","FAMILLE_Validation","")
	
	' forced
	if Doc("FAMILLE").Forced = true then
		Doc("FAMILLE") = "" ' 2.6.3
		ReportForcedIndexDocRD("FAMILLE")		
		FAMILLE_Validation = False
		exit function
	end if		
	
	dim partieGaucheWex, PosTiret, taillePartieGauche, TailleCodePiece
	
	PosTiret = Instr(DOC("FormNameRecto"), "_") 
	TailleCodePiece = Len(DOC("FormNameRecto")) - PosTiret
	
	' On calcule la taille de la partie droite du formulaire WEX
	taillePartieGauche = 5
	
	' On récupére la partie droite du formulaire WEX
	if ( len(DOC("FormNameRecto")) &gt; PosTiret ) then
		Doc("UserField010") = Mid(DOC("FormNameRecto"), PosTiret+1, TailleCodePiece)   ' Code Pièce
		partieGaucheWex = Mid(DOC("FormNameRecto"), 1, taillePartieGauche) ' Partie gauche du Formulaire Wex
	else
		Doc("UserField010") = ""
		partieGaucheWex = ""
	end if

	' Si on est en phase de vidéoCodage et que le champs est vide, alors on propose la liste
	if G("Phase") = "VCD" then
		if(Doc("FAMILLE") = "") then 
			FAMILLE_Validation = False				
		else
			FAMILLE_Validation = true
		end if
	else
		FAMILLE_Validation = True
	end if
		
	checkStatus()  'Vérification du statut du document
	
	'report
	if FAMILLE_Validation then ReportIndexDocRD("FAMILLE")
	
End function

'#################################################
'TYPE_DOC
Function TYPE_DOC_Validation()
	call TraceDebug("fn","TYPE_DOC_Validation","TYPE_DOC : " &amp; doc("TYPE_DOC")) 

	' valid
	if len(doc("TYPE_DOC")) &gt; 0 then
		TYPE_DOC_Validation = True
		MsgErreur = ""
	else
		TYPE_DOC_Validation = False
		'exit function ' 2.6.3
	end if

	' report
	if TYPE_DOC_Validation then
		ReportIndexDocRD("TYPE_DOC")
		ReportIndexDocRD("FAMILLE")
		ReportIndexDocRD("COTE")
		ReportIndexDocRD("CONSERV_ORIGINAL")
		ReportIndexDocRD("VALEUR_JURIDIQUE")
		ReportIndexDocRD("PRIORITE")
	end if
	
	checkStatus()  'Vérification du statut du document
	
	if TYPE_DOC_Validation then reportIndexDocRD("TYPE_DOC")
End function


'#################################################
' COTE
Function COTE_Validation()
    call TraceDebug("fn","COTE_Validation","") 
    COTE_Validation = True
	' report
	if COTE_Validation then ReportIndexDocRD("COTE")
End function


'#################################################
' STATUS
Function STATUS_Validation()
    call TraceDebug("fn","STATUS_Validation","") 
    
    checkStatus
    
    if( len(Doc("STATUS")) &gt; 0 ) then
		STATUS_Validation = True
	else
		STATUS_Validation = false
	end if
	
	checkStatus()  'Vérification du statut du document
	
	' report
	if STATUS_Validation then ReportIndexDocRD("STATUS")
End function

'#################################################
'STATUS_INDEXATION
Function STATUS_INDEXATION_Validation()
    call TraceDebug("fn","STATUS_INDEXATION_Validation","") 
    
    checkStatus()
    
    if( len(Doc("STATUS_INDEXATION")) &gt; 0 ) then
		STATUS_INDEXATION_Validation = True
		G_msg = ""
	else
		STATUS_INDEXATION_Validation = false
		G_msg = "Status d'indexation incorrect"
	end if
	
	checkStatus()  'Vérification du statut du document
	
	' report
	if STATUS_INDEXATION_Validation then ReportIndexDocRD("STATUS_INDEXATION")
End function

'#################################################
' OBSERVATION
Function OBSERVATION_Validation()
    call TraceDebug("fn","OBSERVATION_Validation","") 
    OBSERVATION_Validation = True
	
	' report
	if OBSERVATION_Validation then ReportIndexDocRD("OBSERVATION")
End function

'#################################################
' NATURE
Function NATURE_Validation()
    call TraceDebug("fn","NATURE_Validation","") 
    NATURE_Validation = True
    exit function
End function

'#################################################
' MODE
Function MODE_Validation()
   call TraceDebug("fn","MODE_Validation","") 
   MODE_Validation = True
   
   checkStatus()  'Vérification du statut du document
   
	' report
	if MODE_Validation then ReportIndexDocRD("MODE")
End function

'#################################################
' CODE_BARRE
Function CODE_BARRE_Validation()
	call TraceDebug("wth","CODE_BARRE_Validation","") 
	'call TraceDebug("wth","CODE_BARRE_Validation ","ValCodeBarre : " &amp; Doc("CODE_BARRE"))
	' forced
	if Doc("CODE_BARRE").Forced = true then
		ReportForcedIndexDocRD("CODE_BARRE")		
		CODE_BARRE_Validation = False
		exit function
	end if	
	
		'LECTURE DU CODE BARRE
	'Dim tempcodebarre
	'Dim valCodeBarre
'Si on est en LAD
	if( G("Phase") = "LAD") then
		' et que le code à barre n'est pas vide
		if(Doc("CODE_BARRE") &lt;&gt; "" ) then
			if(getIndexByCodeBarre() = true) then
				MsgErreur = ""
				CODE_BARRE_Validation = true
			else
				MsgErreur = "Code à barre non référencé"
				CODE_BARRE_Validation = false
			end if
		else
			MsgErreur = "Code à barre non lu"
			CODE_BARRE_Validation = false
		end if

	end if

	'Si on est en VCD 
	if( G("Phase") = "VCD") then
		'et que le code à barre est vide
		if( len(Doc("CODE_BARRE")) &gt; 0 ) then
			' Si la récup retourne "true" c'est que l'on a trouvé la ligne
			if(getIndexByCodeBarre() = true) then				
				MsgErreur = ""
				CODE_BARRE_Validation = true
			else
				MsgErreur = "Valeur code à barre non référencée."
				CODE_BARRE_Validation = false
			end if
		end if

	end if

	 ' report
	if CODE_BARRE_Validation then 
		ReportIndexDocRD("CODE_BARRE")
		ReportIndexDocRD("NO_ADHERENT")	
		ReportIndexDocRD("NO_CONTRAT")		
		ReportIndexDocRD("DATE_DOCUMENT")	
		ReportIndexDocRD("NO_PROPOS")		 
		ReportIndexDocRD("LIB_DOC")		
		ReportIndexDocRD("NO_AVENANT")		
		ReportIndexDocRD("MultiCompteId")	
	end if	
	
End function


'=========================================================================================================
' 3.1 - Fonctions de Listes - appelées par la Validation des champs
'=========================================================================================================

'-------------------------------------------------DEV----------------------------------------------------------------
'#################################################
' Liste NO_CONTRAT
Function ListNO_CONTRATPlugin()
	call TraceDebug("fn","ListNO_CONTRATPlugin","") 
	Dim plugin
	Dim nbenreg
	Dim SQL
	Dim ArrayRes ' tableau des champs resultats
	Dim RechercheBase
	
	Dim msg
	Dim retmsg
	Dim titre

	Dim NO_CONTRAT
	Dim noContratSansLettre
	Dim NO_ADHERENT
	
	G_msg=""
	NO_CONTRAT = Doc("NO_CONTRAT")
	NO_ADHERENT = Doc("NO_ADHERENT") 
	
	' On lit le N° de contrat sans la lettre de fin
	noContratSansLettre = readWhileNum(NO_CONTRAT)
   SQL = "(NumeroContrat LIKE '%" &amp; noContratSansLettre &amp; "%' and numeroavenant like '%" &amp; noContratSansLettre &amp; "%')"
   
    ' Si le N° d'adhérent est renseigné et validé
    if(len(NO_ADHERENT) &gt; 0 and Doc("NO_ADHERENT").Valid) then
		G_msg = ""
		SQL = SQL &amp; " AND CompteId = " &amp; NO_ADHERENT &amp; " "
	else
		G_msg = "Adhérent non validé (A forcer)"
		ListNO_CONTRATPlugin = false
		exit function
	end if
     
	call TraceDebug("data","ListNO_CONTRATPlugin",SQL) 
   
   ' Nombre d'enregistrements	
	If DB.RecordCreate("CountNbEnreg","Select count(*) as nbenreg from V_CONTRAT_GECO where " &amp; SQL) then
		nbenreg = DB.RecordValue("CountNbEnreg","nbenreg")
	end if																		
	call TraceDebug("data","ListNO_CONTRATPlugin","nbenreg=" &amp; nbenreg &amp; ", MaxTop=" &amp; MaxTop) 
	if clng(nbenreg) &gt; clng(MaxTop) then
		G_msg = "RAJOUTER PLUS DE CARACTERES NO_CONTRAT (NB: " &amp; nbenreg &amp; ")"
		ListNO_CONTRATPlugin = false
		exit function
	end if
	
	' Affiche la liste (Plugin)
	ListNO_CONTRATPlugin = false
	    
	Set plugin =  AtomLib.InitPlugin("AtomVCDPlugin.SelectList")
	SQL = "Select NumeroContrat ,LibelleProduit FROM  V_CONTRAT_GECO where " &amp; SQL
	
	nbenreg = clng(plugin.Load(SQL))
	titre = "CONTRATS (TOTAL : " &amp; nbenreg &amp; ")"
	if nbenreg &gt; 1 then
		if ( G("Phase") &lt;&gt; "LAD" ) then
			If plugin.Afficher(titre,8000,0) then
				ListNO_CONTRATPlugin = true
			end if
		end if
	elseif nbenreg = 1 then
			ListNO_CONTRATPlugin = true
	else
		if ( G("Phase") &lt;&gt; "LAD" ) then
			G_msg =  "CE CONTRAT EST ABSENT DE LA BASE"
			ListNO_CONTRATPlugin = false
		end if	
	end if

	' Recup enreg
	if (ListNO_CONTRATPlugin = true) then
		ArrayRes = Split(plugin.resultat,"#",-1,1)
'		Doc("NO_CONTRAT") = ArrayRes(0) &amp; ArrayRes(1)
		Doc("NO_CONTRAT") = ArrayRes(0)
	end if
	
End Function

'--------------------------------------------------DEV---------------------------------------------------------------

'=========================================================================================================
' 3.2 - Fonctions de récup de données - appelées par la validation des champs
'=========================================================================================================

'#################################################
Function RecupCHARTEDOC(TYPE_DOC)
	' Pour une CODE_TYPE_DOC : Renvoi Famille, pers a notifier, Conserv ori, cote, etc...
	call TraceDebug("wth","RecupCHARTEDOC", "TYPE_DOC = " &amp; TYPE_DOC) 
	Dim Nombre
	dim Recordset
	dim delai1
	dim delai2
	dim date_delai1
	dim date_delai2
	dim sql
	dim sqlPart
'	Dim IdEspace
	
'	IdEspace = 	getEspaceID(Doc("NO_CONTRAT"))
'	call TraceDebug("wth","RecupCHARTEDOC", "NO_CONTRAT = " &amp; Doc("NO_CONTRAT")) 
'	call TraceDebug("wth","RecupCHARTEDOC", "IdEspace = " &amp; IdEspace) 
	
	G_msg = ""
	Nombre = 0
	RecupCHARTEDOC = false
	
	'count
'	if IdEspace = 3 then
'		sql = "Select Count(*) as Nombre from C_CHARTEDOC where ACTIVITE = 'EMOA1' and type_doc = '" &amp; TYPE_DOC &amp; "'" 
'	else
'		sql = "Select Count(*) as Nombre from C_CHARTEDOC where ACTIVITE = 'GECO1' and type_doc = '" &amp; TYPE_DOC &amp; "'" 
'	end if		
	sql = "Select Count(*) as Nombre from C_CHARTEDOC where type_doc = '" &amp; TYPE_DOC &amp; "'" 

	
	If DB.RecordCreate("CountCHARTEDOC",sql) then
		Nombre = DB.RecordValue("CountCHARTEDOC","Nombre")
	end if
	
	' Récup infos
	if Nombre = 1 then
		Recordset = "CHARTEDOC"

'		if IdEspace = 3 then
'			sql = "Select * from C_CHARTEDOC where ACTIVITE = 'emoa1' and type_doc = '" &amp; TYPE_DOC &amp; "'"  
'		else
'			sql = "Select * from C_CHARTEDOC where ACTIVITE = 'GECO1' and type_doc = '" &amp; TYPE_DOC &amp; "'"  
'		end if		
		sql = "Select * from C_CHARTEDOC where type_doc = '" &amp; TYPE_DOC &amp; "'"  

'		call TraceDebug("wth","RecupCHARTEDOC", "SQL = " &amp; sql) 

		If DB.RecordCreate(Recordset,SQL) then
			DOC("FAMILLE") 			= DB.RecordValue(Recordset,"FAMILLE")
			DOC("COTE") 			= DB.RecordValue(Recordset,"COTE")
			DOC("CONSERV_ORIGINAL")	= DB.RecordValue(Recordset,"CONSERV_ORIGINAL")
			DOC("VALEUR_JURIDIQUE")	= DB.RecordValue(Recordset,"VALEUR_JURIDIQUE")
			DOC("PRIORITE") 		= DB.RecordValue(Recordset,"PRIORITE")
			RecupCHARTEDOC = true
			G_msg = ""
		end if
	else
		G_msg = "CHARTEDOC : Doublons de TYPE_DOC"
	end if
		
End function


'=========================================================================================================
' 3.3 - Fonctions de Report d'index - appelées par la Validation des champs
'=========================================================================================================

'#################################################
Function ReportIndexTrans(Champ)
    call TraceDebug("fn","ReportIndexTrans","") 
	Dim i 'as long
	Dim Valeur ' a string
	Dim TheDoc ' as object
	
	Valeur = ""
	
	for i = 1 to trans.count

		Set TheDoc = TRANS(i)
		If i &lt;&gt; doc.Position and  (TheDoc("TypeDocument") &lt;&gt; "ABC") then
			Select case F.Name 
				case "OPERATEUR"
					' OPERATEUR
					Valeur = DOC("OPERATEUR")
					TheDoc("OPERATEUR") = Valeur
					TheDoc("OPERATEUR").Valid = True
				case "DATE_DOCUMENT"
					Valeur = DOC("DATE_DOCUMENT")
					TheDoc("DATE_DOCUMENT") = Valeur
					TheDoc("DATE_DOCUMENT").Valid = True
				case "PROVENANCE"
					' PROVENANCE
					Valeur = DOC("PROVENANCE")
					TheDoc("PROVENANCE") = Valeur
					TheDoc("PROVENANCE").Valid = True
				case "DESTINATAIRE"
					' DESTINATAIRE
					Valeur = DOC("DESTINATAIRE")
					TheDoc("DESTINATAIRE") = Valeur
					TheDoc("DESTINATAIRE").Valid = True
				case "SERVICE"
					' SERVICE
					Valeur = DOC("SERVICE")
					TheDoc("SERVICE") = Valeur
					TheDoc("SERVICE").Valid = True
				case "NO_ADHERENT"
					' NO_ADHERENT
					Valeur = DOC("NO_ADHERENT")
					TheDoc("NO_ADHERENT") = Valeur
					TheDoc("NO_ADHERENT").Valid = True
				case "NO_CONTRAT"
					' NO_CONTRAT
					Valeur = DOC("NO_CONTRAT")
					TheDoc("NO_CONTRAT") = Valeur
					TheDoc("NO_CONTRAT").Valid = True
				case "CODE_GESTIONNAIRE_REDAC"
					' CODE_GESTIONNAIRE_REDAC
					Valeur = DOC("CODE_GESTIONNAIRE_REDAC")
					TheDoc("CODE_GESTIONNAIRE_REDAC") = Valeur
					TheDoc("CODE_GESTIONNAIRE_REDAC").Valid = True
				case "CODE_REDACTEUR_QUALITE"
					' CODE_REDACTEUR_QUALITE
					Valeur = DOC("CODE_REDACTEUR_QUALITE")
					TheDoc("CODE_REDACTEUR_QUALITE") = Valeur
					TheDoc("CODE_REDACTEUR_QUALITE").Valid = True
				case "CODE_REDACTEUR_VISU"
					' CODE_REDACTEUR_VISU
					Valeur = DOC("CODE_REDACTEUR_VISU")
					TheDoc("CODE_REDACTEUR_VISU") = Valeur
					TheDoc("CODE_REDACTEUR_VISU").Valid = True
				case "CODE_REDACTEUR_TRAITE_DOC"
					' CODE_REDACTEUR_TRAITE_DOC
					Valeur = DOC("CODE_REDACTEUR_TRAITE_DOC")
					TheDoc("CODE_REDACTEUR_TRAITE_DOC") = Valeur
					TheDoc("CODE_REDACTEUR_TRAITE_DOC").Valid = True
				case "DATE_QUALITE"
					' DATE_QUALITE
					Valeur = DOC("DATE_QUALITE")
					TheDoc("DATE_QUALITE") = Valeur
					TheDoc("DATE_QUALITE").Valid = True
				case "DATE_VISU"
					' DATE_VISU
					Valeur = DOC("DATE_VISU")
					TheDoc("DATE_VISU") = Valeur
					TheDoc("DATE_VISU").Valid = True
				case "DATE_TRAITE_DOC"
					' DATE_TRAITE_DOC
					Valeur = DOC("DATE_TRAITE_DOC")
					TheDoc("DATE_TRAITE_DOC") = Valeur
					TheDoc("DATE_TRAITE_DOC").Valid = True
			End select
			
		End if
	next
	
End function

'#################################################
Function ReportForcedIndexTrans(Champ)
	call TraceDebug("fn","ReportForcedIndexTrans","") 
	
	Dim i 			'as long
	Dim Valeur 		' a string
	Dim StatutF 	' a string	
	Dim StatutV 	' a string		
	Dim TheDoc 		' as object
	
	Valeur = ""
	
	for i = 1 to trans.count
		Set TheDoc = TRANS(i)

		If i &lt;&gt; doc.Position and  (TheDoc("TypeDocument") &lt;&gt; "ABC") then
			Select case F.Name 
				case "REF_AUTRE"
					' REF_AUTRE
					Valeur  = DOC("REF_AUTRE")
					StatutF = DOC("REF_AUTRE").Forced
					StatutV = DOC("REF_AUTRE").Valid
					TheDoc("REF_AUTRE") = Valeur
					TheDoc("REF_AUTRE").Valid = StatutV
					TheDoc("REF_AUTRE").Forced = StatutF
				case "PROVENANCE"
					' PROVENANCE
					'Valeur  = DOC("PROVENANCE")
					StatutF = DOC("PROVENANCE").Forced
					StatutV = DOC("PROVENANCE").Valid
					'TheDoc("PROVENANCE") = Valeur
					TheDoc("PROVENANCE").Valid = StatutV
					TheDoc("PROVENANCE").Forced = StatutF
				case "DESTINATAIRE"
					' DESTINATAIRE
					Valeur  = DOC("DESTINATAIRE")
					StatutF = DOC("DESTINATAIRE").Forced
					StatutV = DOC("DESTINATAIRE").Valid
					TheDoc("DESTINATAIRE") = Valeur
					TheDoc("DESTINATAIRE").Valid = StatutV
					TheDoc("DESTINATAIRE").Forced = StatutF
				case "SERVICE"
					' SERVICE
					Valeur  = DOC("SERVICE")
					StatutF = DOC("SERVICE").Forced
					StatutV = DOC("SERVICE").Valid
					TheDoc("SERVICE") = Valeur
					TheDoc("SERVICE").Valid = StatutV
					TheDoc("SERVICE").Forced = StatutF
				case "NO_ADHERENT"
					' NO_ADHERENT
					Valeur  = DOC("NO_ADHERENT")
					StatutF = DOC("NO_ADHERENT").Forced
					StatutV = DOC("NO_ADHERENT").Valid
					TheDoc("NO_ADHERENT") = Valeur
					TheDoc("NO_ADHERENT").Valid = StatutV
					TheDoc("NO_ADHERENT").Forced = StatutF	
				case "NO_CONTRAT"
					' NO_CONTRAT
					Valeur  = DOC("NO_CONTRAT")
					StatutF = DOC("NO_CONTRAT").Forced
					StatutV = DOC("NO_CONTRAT").Valid
					TheDoc("NO_CONTRAT") = Valeur
					TheDoc("NO_CONTRAT").Valid = StatutV
					TheDoc("NO_CONTRAT").Forced = StatutF	
				case "CODE_GESTIONNAIRE_REDAC"
					' CODE_GESTIONNAIRE_REDAC
					Valeur  = DOC("CODE_GESTIONNAIRE_REDAC")
					StatutF = DOC("CODE_GESTIONNAIRE_REDAC").Forced
					StatutV = DOC("CODE_GESTIONNAIRE_REDAC").Valid
					TheDoc("CODE_GESTIONNAIRE_REDAC") = Valeur
					TheDoc("CODE_GESTIONNAIRE_REDAC").Valid = StatutV
					TheDoc("CODE_GESTIONNAIRE_REDAC").Forced = StatutF	
				
			End select

			Select case F.Name 
				case "REF_AUTRE"
					' REF_AUTRE
					TheDoc("REF_AUTRE").Forced = True
					TheDoc("REF_AUTRE").Valid = False
				case "PROVENANCE"
					' PROVENANCE
					TheDoc("PROVENANCE").Forced = True
					TheDoc("PROVENANCE").Valid = False
				case "DESTINATAIRE"
					' DESTINATAIRE
					TheDoc("DESTINATAIRE").Forced = True
					TheDoc("DESTINATAIRE").Valid = False
				case "SERVICE"
					' SERVICE
					TheDoc("SERVICE").Forced = True
					TheDoc("SERVICE").Valid = False
				case "NO_ADHERENT"
					' NO_ADHERENT
					TheDoc("NO_ADHERENT").Forced = True
					TheDoc("NO_ADHERENT").Valid = False
				case "NO_CONTRAT"
					' NO_CONTRAT
					TheDoc("NO_CONTRAT").Forced = True
					TheDoc("NO_CONTRAT").Valid = False	
			End select

			
		End if
	next
	
End function

'#################################################
Function ReportIndexDocRD(Champ)
	Dim i 'as long
	Dim Valeur ' a string
	Dim TheDoc ' as object
	'Dim Champ ' as string
	
	'Champs = F.Name
	Valeur = ""
		
	' on va du doc courant vers le 1er doc jusqu'a rupture
	for i = Doc.Position - 1  to 1 step - 1
		'if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then exit for
		if TRANS.Value(i,"ReferencePaquet") &lt;&gt; Doc("ReferencePaquet") then exit for ' 2.4.7
		Valeur = DOC(Champ)
		Set TheDoc = TRANS(i)
		TheDoc(Champ) = Valeur
		TheDoc(Champ).Valid = True
	next

	' on va du doc courant vers le dernier doc jusqu'a rupture
	for i = Doc.Position + 1  to TRANS.Count
		if TRANS.Value(i,"ReferencePaquet") &lt;&gt; Doc("ReferencePaquet") then exit for ' 2.4.7
		Valeur = DOC(Champ)
		Set TheDoc = TRANS(i)
		TheDoc(Champ) = Valeur
		TheDoc(Champ).Valid = True
	next
	
End function

'#################################################
Function ReportForcedIndexDocRD(Champ)
	Dim i 'as long
	Dim Valeur ' a string
	Dim TheDoc ' as object
	
	Valeur = ""
		
	' on va du doc courant vers le 1er doc jusqu'a rupture
	for i = Doc.Position - 1  to 1 step - 1
		'if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then exit for
		if TRANS.Value(i,"ReferencePaquet") &lt;&gt; Doc("ReferencePaquet") then exit for ' 2.4.7
		Valeur = DOC(Champ)
		Set TheDoc = TRANS(i)
		TheDoc(Champ) = Valeur
		TheDoc(Champ).Forced = True
	next

	' on va du doc courant vers le dernier doc jusqu'a rupture
	for i = Doc.Position + 1  to TRANS.Count
		'if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then exit for
		if TRANS.Value(i,"ReferencePaquet") &lt;&gt; Doc("ReferencePaquet") then exit for ' 2.4.7
		Valeur = DOC(Champ)
		Set TheDoc = TRANS(i)
		TheDoc(Champ) = Valeur
		TheDoc(Champ).Forced = True
	next
	
End function

'20161116 - AZ
'#################################################
Function GetFormattedDate(strDate)

	Dim strDay
	Dim strMonth
	Dim strYear
	
	'strDate = CDate(Date) 'Date du jour
	
	strDay = DatePart("d", strDate)
	strMonth = DatePart("m", strDate)
	strYear = DatePart("yyyy", strDate)
	If strDay &lt; 10 Then
		strDay = "0" &amp; strDay
	End If
	If strMonth &lt; 10 Then
		strMonth = "0" &amp; strMonth
	End If
	
	GetFormattedDate = cstr(strYear &amp; "" &amp; strMonth &amp; "" &amp; strDay)

End function

'=========================================================================================================
' 4 - Fonctions liées à ControlApresLecture - Appelée après LAD
'=========================================================================================================

'#################################################
Function ControlApresLecture()
	call TraceDebug("fn","ControlApresLecture","") 

	Dim FormulaireWex, PosTiret, TailleCodePiece, taillePartieGauche, valLoc, partieGaucheWex
	Dim debRefArchive
	'Dim FormulaireWex, PosTiret, TailleCodePiece, valLoc

	' ReferencePaquet pour la constitution de dossier
	call ConstitutionDocDTPP() ' 2.4.7	
	
	'*** RECUPERATION DU CODE ORIGINE DU FORMULAIRE
	FormulaireWex = DOC("FormNameRecto")

				'##WTH;20151015;V2 

	debRefArchive = Doc("CodeBanque")
	
	' Remplacement dans la REF_ARCHIVE DE 'MAFPJ' par la partie spécifique au scénario (venant de PowerScan)
	Doc("CodeCondition") = replace(Doc("CodeCondition"),"MAFPJ",debRefArchive)
	'codetypedoc'
	Doc("UserField001") = Doc("CodeCondition")
	
	call RangDemandeDansLot() 

	' ----------------- Indexation des contrats -------------
	
	Doc("MODE") 	= Doc("ReferenceBordereau") ' On récupére le mode (ex : COURRIER) du document
	Doc("SERVICE") 	= Doc("ReferenceSession") ' On récupére le service (ex : PROD)
	Doc("DATE_NUM") = Doc("DateTraitement")

'20161116 - AZ
	Doc("DATE_DOCUMENT") = GetFormattedDate(Doc("DateTraitement"))

End function

'#################################################
Function RangDemandeDansLot()
	call TraceDebug("fn","RangDemandeDansLot","") 
	Dim NbEnreg
	Dim Rang
	Dim NbTotal
	Dim SQL

	Dim DateTraitement
	Dim NumeroMachineEntree
	Dim NumeroUT
	Dim NumeroTransaction
	
	NbEnreg	= 0
	Rang   	= 0
	NbTotal	= 0
	
	RangDemandeDansLot = False
		
 	DateTraitement = Doc("DateTraitement")
	NumeroMachinEentree = Doc("NumeroMachineEntree")
	NumeroUT = Doc("NumeroUT")
	NumeroTransaction = Doc("NumeroTransaction")
  
	SQL = "Select count(distinct numerotransaction) as NbEnreg from document" _
		&amp; " where DateTraitement= '" &amp; DateTraitement _
		&amp; "' and NumeroMachineEntree= " &amp; NumeroMachineEntree _
		&amp; " and NumeroUT= " &amp; NumeroUT _
		&amp; " and NumeroTransaction &lt;= " &amp; NumeroTransaction _
		&amp; " and TypeDocument &lt;&gt; 'DOC'" ' 2.4.7
		
	
	If DB.RecordCreate("CountRangDemande", SQL) then
		NbEnreg = DB.RecordValue("CountRangDemande","NbEnreg")
	end if
	
	' rang de la de demande dans le lot
	Rang =  Clng(NbEnreg)
	Doc("UserField008") = Rang

	NbEnreg = 0
	SQL = "Select count(distinct numerotransaction) as NbEnreg from document" _
		&amp; " where DateTraitement= '" &amp; DateTraitement _
		&amp; "' and NumeroMachineEntree= " &amp; NumeroMachineEntree _
		&amp; " and NumeroUT= " &amp; NumeroUT _
		&amp; " and NumeroTransaction &gt; " &amp; NumeroTransaction
	
	If DB.RecordCreate("CountApresDemande", SQL) then
		NbEnreg = DB.RecordValue("CountApresDemande","NbEnreg")
	end if
	
	NbTotal = Clng(Rang) + Clng(NbEnreg)
	
	' Nombre total de demandes dans le lot
	Doc("UserField007") = NbTotal
	
	RangDemandeDansLot = True
	
End function

'#################################################
Function GenerationNumeroDocRD()
	call TraceDebug("fn","GenerationNumeroDocRD","") 
	Dim i 'as long
	Dim TheDoc ' as object
	Dim CpamIndexDoc

   ' on récupère le no de doc du 1er doc du lot
	if ( TRANS.Count &gt; 0 ) then
		Set TheDoc = TRANS(1)
		CpamIndexDoc = TRANS.Value(1,"NumeroDocEntree")
		TheDoc("ReferencePaquet") = CpamIndexDoc
	end if
	
	' on va du doc courant vers le dernier doc jusqu'a rupture
	for i = 2  to TRANS.Count
		Set TheDoc = TRANS(i)
		if (TRANS.Value(i,"FormNameRecto") &lt;&gt; TRANS.Value(i-1,"FormNameRecto"))  then 
			CpamIndexDoc = TRANS.Value(i,"NumeroDocEntree")
		end if
		TheDoc("ReferencePaquet") = CpamIndexDoc
	next
	
End function

'#################################################
Function ConstitutionDocDTPP()
	' 2.4.7
	Dim posdoc
	Dim TheDoc ' as object
	Dim SuivDoc ' as object

	ConstitutionDocDTPP = False
	
	Doc("ReferencePaquet") = Doc("NumeroDocEntree")
	posdoc = Doc.Position
	
	' Champ ReferencePaquet permet de constituer les documents (SEPDOC)(PIECE)*

	if ( (posdoc &gt; 1) and ((Doc("TypeDocument") = "GECO1") ) ) then
		Set TheDoc = TRANS(posdoc-1)
		
		Select case TheDoc("TypeDocument") 
			case "GECO1"
				Doc("ReferencePaquet") = TheDoc("ReferencePaquet")
			case "SEPPLI"
				Doc("ReferencePaquet") = TheDoc("ReferencePaquet")
			case "SEPDOC"
				Doc("ReferencePaquet") = TheDoc("ReferencePaquet")		
			case else
			
		End select		
		
	end if
	ConstitutionDocDTPP = True
End function


'=========================================================================================================
' 5 - Fonctions Générales - Appelées par Atome : Paramétrées dans le Déclaratif 
'=========================================================================================================
'#################################################
Function AffichInfo()
	call TraceDebug("fn","AffichInfo","") 

	Dim BaseBenef

	if (G_ActiveBenef = 0) then
			BaseBenef = "        BASE  : ACTIVE"
	else
			BaseBenef = "        BASE  : DESACTIVE"
	end if	

	AffichInfo = "DATE NUM   : " &amp; DOC("DateTraitement") &amp; BaseBenef &amp; vbcrlf
	AffichInfo = AffichInfo &amp; "NOM DU LOT : " &amp; DOC("UserField001") &amp; vbcrlf
	AffichInfo = AffichInfo &amp; "CODE PIECE : " &amp; DOC("UserField010") &amp; " - " &amp; DOC("UserField009") &amp; vbcrlf &amp; vbcrlf
	
	AffichInfo = AffichInfo &amp; "RANG DE CE PLI DANS LOT : " &amp; DOC("UserField008") &amp; " / " &amp; DOC("UserField007") &amp; vbcrlf
	
	AffichInfo = AffichInfo &amp; "RANG DE CE DOC DANS PLI            : " &amp; RangDocDansEnvoi &amp; vbcrlf
	AffichInfo = AffichInfo &amp; "RANG DE CETTE FEUILLE DANS PLI     : " &amp; cstr(DOC.position) &amp; " / " &amp; trans.count &amp; vbcrlf 
	AffichInfo = AffichInfo &amp; "RANG DE CETTE FEUILLE DANS PLI     : " &amp; RangFeuilleDansPli &amp; vbcrlf ' 2.5.0 

	AffichInfo = AffichInfo &amp; "RANG DE CETTE FEUILLE DANS DOC   : " &amp; PositionDocRD
				
End function

'#################################################
Function RangFeuilleDansPli()
	' 2.5.0
	call TraceDebug("fn","RangFeuilleDansPli","") 
	Dim i 'as long
	Dim Resultat 'as long
	Dim NbDoc 'as long
	Dim PositionActuel 'as long	
	Dim FormActuel 'as long
	Dim FormPrec 'as long
	
	NbDoc = 0

	PositionActuel = Doc.Position
	
	' on va du 1er doc vers le 1er derner sans les SEPDOC
	for i = 1  to TRANS.Count
	
		if TRANS.Value(i,"TypeDocument") &lt;&gt; "SEPDOC" then NbDoc = NbDoc + 1 ' 2.5.0
		
		if i = PositionActuel then
			Resultat = cstr(NbDoc)
		end if
		
	next
	
	Resultat = Resultat &amp; " / " &amp; cstr(NbDoc)
	
	RangFeuilleDansPli = cstr(Resultat)
End function

'#################################################
Function RangDocDansEnvoi()
	call TraceDebug("fn","RangDocDansEnvoi","") 
	Dim i 'as long
	Dim Resultat 'as long
	Dim NbDoc 'as long
	Dim PositionActuel 'as long	
	Dim FormActuel 'as long
	Dim FormPrec 'as long
	
	NbDoc = 1

	PositionActuel = Doc.Position
	
	FormActuel = TRANS.Value(PositionActuel,"ReferencePaquet")	' 2.4.7	
		
	FormPrec = TRANS.Value(1,"ReferencePaquet") ' 2.4.7
	if TRANS.Value(1,"TypeDocument") = "SEPDOC" then NbDoc = NbDoc - 1 ' 2.5.0
	
	' on va du doc courant vers le 1er doc jusqu'a rupture
		for i = 1  to TRANS.Count
		
			if TRANS.Value(i,"ReferencePaquet") &lt;&gt; FormPrec then ' 2.4.7
				FormPrec = TRANS.Value(i,"ReferencePaquet") ' 2.4.7
				if TRANS.Value(i,"TypeDocument") &lt;&gt; "SEPDOC" then NbDoc = NbDoc + 1 ' 2.5.0
				'NbDoc = NbDoc + 1    : 2.5.0 
			end if
			
			if i = PositionActuel then
				Resultat = cstr(NbDoc)
			end if
			
		next
	
	Resultat = Resultat &amp; " / " &amp; cstr(NbDoc)
	
	RangDocDansEnvoi = cstr(Resultat)
End function

'#################################################
Function PositionDocRD()
	call TraceDebug("fn","PositionDocRD","") 

	Dim i 'as long
	Dim Pos 'as long
	Dim Resultat 'as long

	Pos = 1
	
	' on va du doc courant vers le 1er doc jusqu'a rupture
	for i = Doc.Position - 1  to 1 step - 1
		if TRANS.Value(i,"ReferencePaquet") &lt;&gt; Doc("ReferencePaquet") then exit for ' 2.4.7
		Pos = Pos + 1
	next

	Resultat = cstr(Pos)
	
	for i = Doc.Position + 1  to TRANS.Count
		if TRANS.Value(i,"ReferencePaquet") &lt;&gt; Doc("ReferencePaquet") then exit for ' 2.4.7
		Pos = Pos + 1
	next

	Resultat = Resultat &amp; " / " &amp; cstr(Pos)
	
	PositionDocRD = cstr(Resultat)
	
End function

'#################################################
' CRTL F5
Function DupliqueChamp()

 Dim retour

	retour = DupliqueChampPrec(F.Name)
	
	Doc(F.Name) = retour
	
	'DupliqueChamp = retour

End Function

'#################################################
Function DupliqueChampPrec(Champ)
	Dim i 'as long
	Dim Valeur ' a string
	Dim TheDoc ' as object
	
	Valeur = ""
		
	if Doc.Position &gt; 1 then 
		i = Doc.Position - 1
		' cas SEPDOC 2.5.0
		if (i &gt; 1) and TRANS.Value(i,"TypeDocument")  = "SEPDOC" then
			i = i - 1
		end if
	else
		i = 1
	end if

	Set TheDoc = TRANS(i)	
	Valeur = TheDoc(Champ)
	
	DupliqueChampPrec = Valeur
	
End function

'=========================================================================================================
' 10 - Fonctions Outils
'=========================================================================================================
'#################################################
Function UserNameWindows()
	' Renvoi le nom d'utilisateur windows 2.5.0
	Dim wshNetwork
	Dim NomUser
	
	call TraceDebug("fn","UserNameWindows","") 
	Set wshNetwork = CreateObject("Wscript.Network")
	NomUser = ucase(wshNetwork.UserName)
	call TraceDebug("data","UserNameWindows", NomUser) 
	
	UserNameWindows = NomUser
End Function

'#################################################
Function TraceDebug(TypeTrace, fonction, Chaine)
	' Ecrit une trace dans le fichier de log
	Const ForAppend = 8
	dim fso, f   
	dim DateJour
	dim strTmp
	dim DocumentEnCours
	dim Phase
   
   ' si on est pas en mode Debug quitter pour ne pas écrire de trace
	'=&gt; ACTIVATION DES TRACES
	'exit function
   
   ' pour les variable non encore déclarées
	on error resume next 

	' Préparation écriture
	DateJour = now   ' DateJour contient la date système actuelle.
	DocumentEnCours = DOC("DateTraitement") &amp; "-" &amp; DOC("NumeroMachineEntree") &amp; "-" &amp; DOC("NumeroDocEntree")
	Phase =  G("Phase")
	strTmp = DateJour &amp; ";Phase=" &amp; Phase &amp; ";DocEncours=" &amp; DocumentEnCours &amp; ";" &amp; TypeTrace &amp; ";" &amp; fonction &amp; ";" &amp; Chaine &amp; vbCrLf

   ' Ouverture
	Set fso = CreateObject("Scripting.FileSystemObject")
	Set f = fso.OpenTextFile("D:\IRIS\ENV\GECO1.log", ForAppend, true)
   
	' Ecriture conditionnelle
	Select case TypeTrace
		' passage dans les fonctions
		case "fn"
			f.write(strTmp)
		' Ecriture des données en cours
		case "data"
			'f.write(strTmp)
		' Ecriture Debugage
		case "wth"
			f.write(strTmp)
	End select
	
	on error goto 0
	
End Function

'#################################################
' Check a string if they are only DIGIT's
'   Return TRUE for only DIGIT
' v.2.6.6
'#################################################
Function IsOnlyDigit(ImputStr) 
Dim i 
    IsOnlyDigit = True
    For i = 1 To Len(ImputStr)
        If InStr(1, "0123456789", Mid(ImputStr, i, 1)) &lt; 1 Then
            IsOnlyDigit = False
            Exit Function
        End If
    Next 
    
End Function
  

'=========================================================================================================
' 99 - Fonctions Appelées au DocExit
'=========================================================================================================
'#################################################
Function DocExit()

	DocExit = True
call TraceDebug("fn","DocExit0 G Phase :",G("Phase")) 
	if ( G("Phase") &lt;&gt; "LAD" ) then
	call TraceDebug("fn","DocExit1 G Phase :",G("Phase")) 
		' Compte le nbre de page du Document
		CountPAGE()
		ReportIndexDocRD("PAGE")
		Doc("OPERATEUR")=UserNameWindows() ' 2.5.2
		ReportIndexDocRD("OPERATEUR") ' 2.5.2
		
	end if
	
	' Action à effectuer en sortie de traitement d'un doc
	if ( G("Phase") = "VCD" ) then
	call TraceDebug("fn","DocExit2 G Phase :",G("Phase")) 
		' Vérifie le statut d'indexation d'un document
		checkStatus()
		
		if DOC("FormNameRecto") &lt;&gt; "PAGE_SUIV" then
			TopEnvoiDocumentCompostage(Doc("CODE_BARRE"))
		end if
	
	end if

End Function

'#################################################
Function CountPAGE()
	' 2.5.0 : on tient compte des doc annulés
	Dim i 'as long
	Dim Valeur ' a string
	Dim TheDoc ' as object
	Dim nbpage 
	Valeur = ""
	nbpage = 1
	' on va du doc courant vers le 1er doc jusqu'a rupture
	for i = Doc.Position  to 1 step - 1
		if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then
		exit for
		end if
		nbpage = nbpage + 1 
	next

	' on va du doc courant vers le dernier doc jusqu'a rupture
	for i = Doc.Position + 1  to TRANS.Count
		if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then 
		exit for
		end if
		nbpage = nbpage + 1 
	next
	
	' Nbpage du Doc
	DOC("PAGE") = nbpage
End function

'-------------------------------------------------------------------------------------------------------
'-------------------------------------------------------------------------------------------------------
'-------------------------------------------------------------------------------------------------------
'                                                CODE92
'-------------------------------------------------------------------------------------------------------
'-------------------------------------------------------------------------------------------------------
'-------------------------------------------------------------------------------------------------------

'#################################################
function MyIsDate(JJ, MM, AAAA)

	if JJ &lt; 1 or JJ &gt; 31 or MM &lt; 1 or MM &gt; 12 then
		MyIsDate = false
		exit function
	end if
	
	if MM = 2 then
		if JJ &gt; 29 then
			MyIsDate = false
			Exit Function
		elseif JJ = 29 then
			if (AAAA mod 4 = 0 and AAAA mod 100 &lt;&gt; 0) or (AAAA mod 400 = 0) then 
				MyIsDate = true
			else
				MyIsDate = false
			end if
			Exit Function
		end if
		MyIsDate = true
	elseif JJ = 31 and (MM=4 or MM=6 or MM=9 or MM = 11) then
		MyIsDate  = false
		Exit Function
	else
		MyIsDate  = true
	end if
	
'	MyIsDate  = true
	
end function

'#################################################
'Function Modulo(Source As String, themodulo As Long) As Long
Function Modulo(Source, themodulo )
    Dim i ' As Long
    Dim d ' As Long
    
    For i = 1 To Len(Source)
        d = d * 10 + (Clng(Mid(Source, i, 1)))
        d = d Mod themodulo
    Next
    
    Modulo = d
    
End Function

'#################################################
Function CleModulo(Source, themodulo )
    Dim complement ' As Long

	complement = themodulo - Modulo(Source, themodulo )

	CleModulo = complement
	
End Function

'#################################################
Function EffaceIndexDemande()

	EffaceIndexTrans()

End Function

'#################################################
Function ForcageDemande()

	if (G_ForceTrans = 0) then
		ForcageTrans(1)
		G_ForceTrans = 1
	else
		ForcageTrans(0)
		G_ForceTrans = 0
	end if	

End Function

'#################################################
Function DesactiveActiveBenef()

	if (G_ActiveBenef = 0) then
		G_ActiveBenef = clng(1)
	else
		G_ActiveBenef = clng(0)
	end if	

End Function

'#################################################
Function MiseAJourCategorie()

	'If doc.position = 1 then
		If len(trans.value(1,"TypeCM")) &gt; 0 then
			if left(right(trans.value(1,"Categorie"),2),1) &lt;&gt; "-" then
				Doc("Categorie") = trans.value(1,"Categorie") &amp; "-" &amp; left(trans.value(1,"TypeCM"),1)
			end if
		end if
		'msgbox("Mise à jour categorie")
		ReportIndexCategorie()
	'end if
	MiseAJourCategorie = True
	
End function

'#################################################
Function ReportIndexCategorie()
	Dim i 'as long
	Dim Valeur ' a string
	Dim TheDoc ' as object
	
	Valeur = ""
	'Msgbox("Reportindexcategorie")
	'MsgBox DOC.position
			
	for i = 1 to trans.count

		Set TheDoc = TRANS(i)

			'Select case Champ
				'case "Categorie"
					Valeur = trans.value(1,"Categorie")
					TheDoc("Categorie") = Valeur
					TheDoc("Categorie").Valid = True
			'	case else
					
			'End select
	next
End function

'#################################################
Function EffaceIndexDocRD()
	Dim i 'as long
	Dim Valeur ' a string
	Dim TheDoc ' as object
	
	Valeur = ""
		
	' on va du doc courant vers le 1er doc jusqu'a rupture
	for i = Doc.Position - 1  to 1 step - 1
		if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then exit for
		Set TheDoc = TRANS(i)
		TheDoc("TypeEmetteur") = Valeur
		TheDoc("TypeEmetteur").Valid = False
		TheDoc("IDEmetteur") = Valeur
		TheDoc("IDEmetteur").Valid = False
		TheDoc("NomEmetteur") = Valeur
		TheDoc("NomEmetteur").Valid = False
		TheDoc("PrenomEmetteur") = Valeur
		TheDoc("PrenomEmetteur").Valid = False
		TheDoc("DateEtablissement") = Valeur
		TheDoc("DateEtablissement").Valid = False		
	next

	' on va du doc courant vers le dernier doc jusqu'a rupture
	for i = Doc.Position to TRANS.Count
		if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then exit for
		Set TheDoc = TRANS(i)
		TheDoc("TypeEmetteur") = Valeur
		TheDoc("TypeEmetteur").Valid = False
		TheDoc("IDEmetteur") = Valeur
		TheDoc("IDEmetteur").Valid = False
		TheDoc("NomEmetteur") = Valeur
		TheDoc("NomEmetteur").Valid = False
		TheDoc("PrenomEmetteur") = Valeur
		TheDoc("PrenomEmetteur").Valid = False
		TheDoc("DateEtablissement") = Valeur
		TheDoc("DateEtablissement").Valid = False		
	next
	
End function



'===================================================================================================
'=======================Fonctions spécifiques rédigées par DEPASSE Johnny ==========================
'===================================================================================================

'#################################################
' Fonction : getDataByAdherent 
' Description : Cette fonction récupére la provenance du client depuis le numéro d'adhérent
' mais aussi le gestionnaire rédacteur associé, et le GROUPE et Service du gestionnaire rédacteur
function getDataByAdherent(noAdh)

	call TraceDebug("fn","getDataByAdherent",noAdh) 
	dim NombreRedac
	dim Recordset
	
	G_msg = ""
	NombreRedac= 0
	
	' Count
	If DB.RecordCreate("CountVALLADH","Select Count(*) as NombreRedac from GEDMAF.dbo.V_ALL_ADHERENT where NO_CLIENT = '" &amp; noAdh &amp; "'") then
		NombreRedac = DB.RecordValue("CountVALLADH","NombreRedac")
	end if
	
	' Récup infos
	if NombreRedac = 1 then
		Recordset = "VALLADH"
		If DB.RecordCreate(Recordset,"Select * from GEDMAF.dbo.V_ALL_ADHERENT where NO_CLIENT = '" &amp; noAdh &amp; "'") then
			DOC("PROVENANCE") = DB.RecordValue(Recordset,"FULL_NAME")
			DOC("SERVICE") = DB.RecordValue(Recordset,"SERVICE")
			getDataByAdherent = true
			G_msg = ""
		end if
	else
		G_msg = "V_ALL_ADHERENT : Doublons de NO_ADHERENT"
		getDataByAdherent = false
	end if

end function

'#################################################
function getDataByAdherentV2(noAdh)

	call TraceDebug("fn","getDataByAdherentV2",noAdh) 
	dim NombreRedac
	dim Recordset
	
	G_msg = ""
	NombreRedac= 0
	
	' Count
	If DB.RecordCreate("CountVALLADH","Select Count(*) as NombreRedac from GEDMAF.dbo.V_ALL_ADHERENT where NO_CLIENT = '" &amp; noAdh &amp; "'") then
		NombreRedac = DB.RecordValue("CountVALLADH","NombreRedac")
	end if
	
	' Récup infos
	if NombreRedac = 1 then
		Recordset = "VALLADHV2"
		If DB.RecordCreate(Recordset,"Select * from GEDMAF.dbo.V_ALL_ADHERENT where NO_CLIENT = '" &amp; noAdh &amp; "'") then
			DOC("PROVENANCE") = DB.RecordValue(Recordset,"FULL_NAME")
			getDataByAdherentV2 = true
			G_msg = ""
		end if
	else
		G_msg = "V_ALL_ADHERENT : Doublons de NO_ADHERENT"
		getDataByAdherentV2 = false
	end if

end function

'#################################################
function getIndexByCodeBarre()
	call TraceDebug("wth","getIndexByCodeBarre","") 
	
	dim valCodeBarre
	dim reqSqlSelect
	dim reqSqlCount
	dim nbRes
	dim nbResRce
	dim libdoc
	dim locCodeTypeDoc
	dim numImgRecto
	call TraceDebug("wth","getIndexByCodeBarre ValCodeBarre","FAMILLE : " &amp; Doc("CODE_BARRE"))
	valCodeBarre = Doc("CODE_BARRE")
	
	dim reqSql

	reqSql = "SELECT EnvoiId,CompteId, CONVERT(VARCHAR,DocumentDate,112) as DATE_DOC, NumeroContrat, DocumentDescription, PropositionID, NumeroAvenant, MultiCompteId  "
	reqSql = reqSql &amp; " FROM GEDMAF.Editions.EnvoiDocumentCompostage WHERE DocumentId = " &amp; valCodeBarre
	reqSqlCount = "SELECT COUNT(*) as Nombre FROM GEDMAF.Editions.EnvoiDocumentCompostage WHERE DocumentId = " &amp; valCodeBarre 
	
		' Count
		If DB.RecordCreate("countNbDocs",reqSqlCount) then
			nbRes = DB.RecordValue("countNbDocs","Nombre")
		end if
		
		if(nbRes &gt; 0) then
			if DB.RecordCreate("GETDOCS",reqSql) then 
				Doc("NO_ADHERENT")		= DB.RecordValue("GETDOCS","CompteId")
				Doc("NO_CONTRAT")		= DB.RecordValue("GETDOCS","NumeroContrat")

				'20161116 - AZ				
				'Doc("DATE_DOCUMENT")	= DB.RecordValue("GETDOCS","DATE_DOC")
				
				Doc("NO_PROPOS")		= DB.RecordValue("GETDOCS","PropositionID")
				libdoc = replace (DB.RecordValue("GETDOCS","DocumentDescription")," signé","")
				Doc("LIB_DOC")			= Replace(libdoc, "°", "")  &amp; " signé"
				Doc("NO_AVENANT")		= DB.RecordValue("GETDOCS","NumeroAvenant")
				Doc("MultiCompteId")	= DB.RecordValue("GETDOCS","MultiCompteId")
				'Remplir Famille, cote et typeDoc
				call FillTriptiques()
				MsgErreur = ""
				getIndexByCodeBarre = true
			end if
		else
			MsgErreur = "Code barre non trouvé dans la BDD. Resaisir ou forcer"
			getIndexByCodeBarre = false
		end if
end function


'#################################################
'Fonction : isValidField
'Détails : Cette fonction return true si un champ est validé ou false si celui ci est invalide ou forcé
function isValidField(anyDocField)
	call TraceDebug("fn","isValidField","") 

	' Si le champ est valide &amp; pas forcé, on retourne true
	if(anyDocField.Valid and anyDocField &lt;&gt; "") then 
		isValidField = True
	else 
		isValidField = False
	end if

end function

'#################################################
'Fonction : checkStatut 
'Détails : Cette fonction applique le statut( En ged ou en corbeille), en fonction du renseignement ou non des autres champs
function checkStatus()
	call TraceDebug("fn","checkStatus","") 
	
	Doc("STATUS") 				= "TORECORD"
	Doc("STATUS_INDEXATION") 	= "INDEXE"

end function

'#################################################
' Fonction alertInVcd 
' Détails: Cette fonction permet d'afficher un "Alert" durant la phase de video-codage.
' Note : Si un MsgBox est demandé durant les phase de RAD et LAD, la phase ne peut aboutir.
' C'est pourquoi on controle que l'on est  bien en phase VCD
function alertInVcd(mesg)
	call TraceDebug("fn","alertInVcd","") 
	if( G("Phase") = "VCD") then
		MsgBox(mesg)
	end if
	Exit function
end function

'#################################################
' Fonction readWhileNum 
' Cette fonction retourne la partie numérique du NO_IDENT lu en LAD (début jusqu'a ce que l'on tombe sur un non num.
function readWhileNum(myString)
	call TraceDebug("fn","readWhileNum","") 

	dim strToReturn
	dim i
	
	strToReturn = ""
	
	if( len(myString) &gt; 0 ) then
		' Pour chaque caractére de la chaine
		for i = 1 to len(myString)	
			' Si caractére par caractére on a bien des numéros
			if( isOnlyDigit( Mid(myString,i,1) ) ) then
				strToReturn = strToReturn &amp; Mid(myString,i,1)
			' Si on tombe sur un caractére, on sort de la boucle FOR
			else
				i = len(myString)
			end if
		next
	end if

	readWhileNum = strToReturn
	
end function

'#################################################
' Fonction isGoodContratNumber 
' Permet de dire si un numéro de contrat est correct ou pas.
'	 - Donc fini par une lettre
function isGoodContratNumber(noContrat)
	call TraceDebug("fn","isGoodContratNumber","")
 
	' Si le dernier caractére du numéro de contrat est bien une lettre
	if(len(noContrat) - len(readWhileNum(noContrat)) = 1) then
		G_msg = ""
		isGoodContratNumber = True
	else
		G_msg = "Le numéro de contrat a été mal lu"
		isGoodContratNumber = False
	end if

end function

'#################################################
' Fonction checkContratInBdd
' Vérifie qu'un contrat est bien en BDD et est bien associé à l'adhérent lu
' Function modifie le 02/04/2015 pour branchement au NSI (Table GECO)
function checkContratInBdd()
	call TraceDebug("fn","checkContratInBdd","")
	
	dim noAdh
	dim Nombre
	dim noCt
	dim reqSql
	
	'On récupére le N° de contrat, sans la lettre de contrôle
	
	'noCt = mid (Doc("NO_CONTRAT"), 0, len(Doc("NO_CONTRAT")) - 1 )
	noCt = readWhileNum(Doc("NO_CONTRAT"))
	noAdh = Doc("NO_ADHERENT")
	
	if(len(noCt) &gt; 0 and len(noAdh) &gt;0) then
	
		Nombre = 0
		
		' On compte le nombre de contrats pour cet adhérent
		If DB.RecordCreate("countCt","Select Count(*) as Nombre from V_CONTRAT_GECO where (numerocontrat like '%" &amp; noCt &amp;"%' OR numeroavenant like '%" &amp; noCt &amp;"%') AND compteid = '" &amp; noAdh &amp; "'") then
			Nombre = DB.RecordValue("countCt","Nombre")
		end if
		
		'Si au moins un contrat ==&gt; La LAD est correcte
		if (Nombre &gt; 0) then 
			checkContratInBdd = true
		else
			checkContratInBdd = False
		end if
	else
		checkContratInBdd = False
	end if
		
end function

'#################################################
' Fonction FillTriptiques

function FillTriptiques()
	call TraceDebug("fn","FillTriptiques","")
		If InStr(1, DOC("LIB_DOC"), "Questionnaire") = 1 Then
		DOC("TYPE_DOC") 		= "QUESTIONNAIRE TECHNIQUE"
		DOC("COTE") 			= "SOUSCRIPTION"
		else
		DOC("TYPE_DOC") 		= "CONDITIONS PARTICULIERES"
		DOC("COTE") 			= "PIECES CONTRACTUELLES"
		call SetTraite()
		end if
		
		DOC("FAMILLE") 			= "DOCUMENTS CONTRAT"
		ReportIndexDocRD("TYPE_DOC")
		ReportIndexDocRD("FAMILLE")
		ReportIndexDocRD("COTE")
		
		FillTriptiques = true
end function

'#################################################
' Fonction SetTraite

function SetTraite()
	call TraceDebug("fn","SetTraite","")
		DOC("CODE_REDACTEUR_QUALITE") 		= DOC("CodeClient")
		DOC("CODE_REDACTEUR_VISU") 			= DOC("CodeClient")
		DOC("CODE_REDACTEUR_TRAITE_DOC") 	= DOC("CodeClient")
		Doc("DATE_VISU") 					= right(Doc("DateTraitement"),4) &amp; right(left(Doc("DateTraitement"),5),2) &amp; left(Doc("DateTraitement"),2)
		Doc("DATE_QUALITE") 				= Doc("DATE_VISU")
		Doc("DATE_TRAITE_DOC") 					= Doc("DATE_VISU")
		ReportIndexDocRD("CODE_REDACTEUR_QUALITE")
		ReportIndexDocRD("CODE_REDACTEUR_VISU")
		ReportIndexDocRD("CODE_REDACTEUR_TRAITE_DOC")
		ReportIndexDocRD("DATE_VISU")
		ReportIndexDocRD("DATE_QUALITE")
		ReportIndexDocRD("DATE_TRAITE_DOC")
		
		SetTraite = true
end function

'#################################################
' Fonction getEspaceID
' Récupere l'id de l'espace du contrat passé en parametre
function getEspaceID(typerecherche, reference)
	call TraceDebug("fn","getEspaceID", "reference : " &amp; reference &amp; " -- Type recherche : " &amp; typerecherche) 

	dim typedoc
	dim Recordset
	dim nbtypedoc
	dim IdEspace
	dim sql
	
	G_msg = ""
	nbtypedoc= 0
	
	select case typerecherche
		case "CONTRAT" : sql = "Select Count(*) as NombreTypeDoc from V_CONTRAT_GECO where NumeroContrat = '" &amp; reference &amp; "'"
		case "PROPOSITION" : sql = "Select Count(*) as NombreTypeDoc from V_PROPOSITION_GECO where NumeroProposition = '" &amp; reference &amp; "'"
	end select 
	
	' Count
	If DB.RecordCreate("CountTYPEDOC",sql) then
		nbtypedoc = DB.RecordValue("CountTYPEDOC","NombreTypeDoc")
	end if
	
	' Récup infos
	if nbtypedoc = 1 then
		Recordset = "selectEspaceID"
		
		select case typerecherche
			case "CONTRAT" : sql = "Select EspaceID from V_CONTRAT_GECO where NumeroContrat = '" &amp; reference &amp; "'"
			case "PROPOSITION" : sql = "Select EspaceID from V_PROPOSITION_GECO where NumeroProposition = '" &amp; reference &amp; "'"
		end select 
		
		If DB.RecordCreate(Recordset,sql) then
			IdEspace = DB.RecordValue(Recordset,"EspaceID")
			getEspaceID = IdEspace
			G_msg = ""
		end if
	else
		select case typerecherche
			case "CONTRAT" : G_msg = "getEspaceID : Doublons sur numero de contrat " &amp; reference
			case "PROPOSITION" : G_msg = "getEspaceID : Doublons sur numero de proposition " &amp; reference
		end select
		
		getEspaceID = 0
	end if

end function

'#################################################
' Fonction TopEnvoiDocumentCompostage
' Permet de posutionner le booleen Received = true sur un document dans base Editions
' V1 THERRIEN WILFRED 14/04/2014
function TopEnvoiDocumentCompostage(IdDoc)
	call TraceDebug("fn","TopEnvoiDocumentCompostage","IdDoc : " &amp; IdDoc) 
	
	dim nbdoc
	dim nbDocRce
	Dim dateNum
	dateNum = Now 
	G_msg = ""
	nbdoc= 0
	
	' Count
'	If DB.RecordCreate("CountDoc","Select Count(*) as NombreDoc from LNKPARBDD01_RW.Editions.dbo.EnvoiDocumentCompostage AS EnvoiDocumentCompostage_1 where DocumentId = '" &amp; IdDoc &amp; "' and IsReceived = 'False'") then
	If DB.RecordCreate("CountDoc","Select Count(*) as NombreDoc from GEDMAF.Editions.EnvoiDocumentCompostage AS EnvoiDocumentCompostage_1 where DocumentId = '" &amp; IdDoc &amp; "' and IsReceived = 'False'") then
		nbdoc = DB.RecordValue("CountDoc","NombreDoc")
		TopEnvoiDocumentCompostage = true
	end if

	If DB.RecordCreate("countNbRce","SELECT COUNT(*) as Nombre FROM GEDMAF.Editions.EnvoiDocumentCompostage WHERE DocumentId = '" &amp; IdDoc &amp; "' and IsReceived = 'True'") then
			nbDocRce = DB.RecordValue("countNbRce","Nombre")
	end if
	if nbdoc = 1 then
'		If DB.Execute("Update LNKPARBDD01_RW.Editions.dbo.EnvoiDocumentCompostage SET IsReceived='True' where DocumentId = '" &amp; IdDoc &amp; "'") then
		If DB.Execute("Update GEDMAF.Editions.EnvoiDocumentCompostage SET IsReceived='True', CodeTypeDoc='GE001', DateNumerisation = '"&amp; dateNum &amp;"' where DocumentId = '" &amp; IdDoc &amp; "'") then
			TopEnvoiDocumentCompostage = true
			G_msg = ""
		end if
	else
	if nbDocRce = 1 then
			DB.Execute("Update GEDMAF.Editions.EnvoiDocumentCompostage SET IsReceived='True', WorkflowId = NULL, CodeTypeDoc='GE001', DateNumerisation = '"&amp; dateNum &amp;"' where DocumentId = '" &amp; IdDoc &amp; "'")
		
	else
		G_msg = "TopEnvoiDocumentCompostage : Doublons de DOC avec " &amp; IdDoc
		TopEnvoiDocumentCompostage = false
	end if 
		
	end if

end function

'#################################################

' Liste ListCODE_GESTIONNAIRE_REDAC_Plugin
Function ListCODE_GESTIONNAIRE_REDAC_Plugin()
	call TraceDebug("fn","ListCODE_GESTIONNAIRE_REDAC_Plugin","") 
	Dim plugin
	Dim nbenreg
	Dim SQL
	Dim ArrayRes ' tableau des champs resultats
	Dim RechercheBase
	
	Dim msg
	Dim retmsg
	Dim titre

	Dim CodeGestionnaireRedac
	
	G_msg=""
	CodeGestionnaireRedac = Doc("CODE_GESTIONNAIRE_REDAC")
	'RequeteListeCODE_GESTIONNAIRE_REDAC = "select code_redacteur, Gestionnaire_Redacteur from V_REDAC_DO order by code_redacteur"
	
	' On lit le N° de CodeGestionnaireRedac
   ' SQL = "code_redacteur LIKE '" &amp; CodeGestionnaireRedac &amp; "%'"

   SQL = "code_redacteur = '" &amp; CodeGestionnaireRedac &amp; "'"
   
	G_msg = ""
	'SQL = SQL &amp; " AND CompteId = " &amp; NO_ADHERENT &amp; " "
     
	call TraceDebug("data","ListCODE_GESTIONNAIRE_REDAC_Plugin",SQL) 
   
   ' Nombre d'enregistrements	
	If DB.RecordCreate("CountNbEnreg","Select count(*) as nbenreg from V_GESTREDAC_PROD where " &amp; SQL) then
		nbenreg = DB.RecordValue("CountNbEnreg","nbenreg")
	end if																		
	call TraceDebug("data","ListCODE_GESTIONNAIRE_REDAC_Plugin","nbenreg=" &amp; nbenreg &amp; ", MaxTop=" &amp; MaxTop) 
	if clng(nbenreg) &gt; clng(MaxTop) then
		G_msg = "RAJOUTER PLUS DE CARACTERES CODE (NB: " &amp; nbenreg &amp; ")"
		ListCODE_GESTIONNAIRE_REDAC_Plugin = false
		exit function
	end if
	
	' Affiche la liste (Plugin)
	ListCODE_GESTIONNAIRE_REDAC_Plugin = false
	    
	Set plugin =  AtomLib.InitPlugin("AtomVCDPlugin.SelectList")
	SQL = "Select code_redacteur FROM  V_GESTREDAC_PROD where " &amp; SQL
	
	nbenreg = clng(plugin.Load(SQL))
	' titre = "CODE (TOTAL : " &amp; nbenreg &amp; ")"
	titre = "[" &amp; nbenreg &amp; "] Codes (Total)"
	
	if nbenreg &gt; 1 then
		if ( G("Phase") &lt;&gt; "LAD" ) then
			If plugin.Afficher(titre,8000,0) then
				ListCODE_GESTIONNAIRE_REDAC_Plugin = true
			end if
		end if
	elseif nbenreg = 1 then
			ListCODE_GESTIONNAIRE_REDAC_Plugin = true
	else
		if ( G("Phase") &lt;&gt; "LAD" ) then
			G_msg =  "CE CODE EST ABSENT DE LA BASE"
			ListCODE_GESTIONNAIRE_REDAC_Plugin = false
		end if	
	end if

	' Recup enreg
	if (ListCODE_GESTIONNAIRE_REDAC_Plugin = true) then
		ArrayRes = Split(plugin.resultat,"#",-1,1)
		Doc("CODE_GESTIONNAIRE_REDAC") = ArrayRes(0)
	end if
	
End Function

'#################################################
function msgboxwil(message)
	if G("Phase") = "VCD" then
		msgbox(message)
	end if
	msgboxwil = true
end function
'#################################################
