' PAPS1.XML
' Mutuelle des Architectes Francais Assurance / Service Production
' Base : xx V1.0 (Basé sur CPDAP.XML)
' Auteur : Depasse Johnny
'-----------------------------------------------------------------------------------------
' 1.0.0	:	27/06/2011	: 	DEJ Création du XML par duplication de CPDAP.xml
' 2.0.0	:	28/05/2013	:	P9/C9	:	WT	:	Adaptation pour Code Gestionnaire Redac Si TYPE_DOC = "NOTE HONORAIRES"
'11.0.0	:	21/10/2013	:	PAPSV11	:	WT	:	adaptation pour Code Gestionnaire Redac Si TYPE_DOC = "BORDEREAU DE REGLEMENT"
'12.0.0 :	19/12/2013	:   PAPSV12 :	WT	:	Correction d'un bug sur la recherche de sinistre dans v_affaire_paps
'-----------------------------------------------------------------------------------------
' TODO
'  - 
'-----------------------------------------------------------------------------------------

Option Explicit

Dim MsgErreur
Dim G_suppverso, G_priorite
Dim G_msg
Dim MaxTop
Dim G_ForceTrans
Dim G_ActiveBenef

Dim siniFounded
Dim noAdhOk, anneeOk

' ----- Variables globales spécifiques PAPS ---
Dim libOrigine
'Dim codeGestionnaireFound


MaxTop = 100
G_ForceTrans = 0
G_ActiveBenef = clng(0)

'=========================================================================================================
' 1 - Fonctions des requetes ValidationDef
'=========================================================================================================


Function RequeteListeFAMILLE()
	call TraceDebug("fn","RequeteListeFAMILLE","") 
	'RequeteListeTYPE_DOC = "select distinct(TYPE_DOC) from C_CHARTEDOC where COTE = 'DAP' AND FILTRE = '1' order by TYPE_DOC"
	
	' MAJ 1.1.0 --
	RequeteListeFAMILLE = "select distinct(FAMILLE) from C_CHARTEDOC where FILTRE = '1' AND ACTIVITE LIKE '%PAPS%' order by FAMILLE"
End function

Function RequeteListePROVENANCE()
	call TraceDebug("fn","RequeteListePROVENANCE","") 
	'RequeteListeTYPE_DOC = "select distinct(TYPE_DOC) from C_CHARTEDOC where COTE = 'DAP' AND FILTRE = '1' order by TYPE_DOC"
	
	' MAJ 1.1.0 --
	RequeteListePROVENANCE = "select distinct(PROVPAPS) from V_SOPEPROV order by PROVPAPS"
End function


'Function RequeteListeTYPE_DOC()
'	call TraceDebug("fn","RequeteListeTYPE_DOC","") 
'	'RequeteListeTYPE_DOC = "select distinct(TYPE_DOC) from C_CHARTEDOC where COTE = 'DAP' AND FILTRE = '1' order by TYPE_DOC"
'	
'	' MAJ 1.1.0 --
'	RequeteListeTYPE_DOC = "select distinct(TYPE_DOC) from C_CHARTEDOC where FILTRE = '1' AND ACTIVITE LIKE '%PAPS%' order by TYPE_DOC"
'End function


Function RequeteListeTYPE_DOC()
	dim codeOri
	
	if ( G("Phase") = "VCD" ) then
		codeOri = Doc("CODE_ORIGINE")
	else
		codeOri = ""
	end if	

	call TraceDebug("fn","RequeteListeTYPE_DOC","") 
	'RequeteListeTYPE_DOC = "select distinct(TYPE_DOC) from C_CHARTEDOC where COTE = 'DAP' AND FILTRE = '1' order by TYPE_DOC"
	
	if (codeOri &lt;&gt; "" ) then
		RequeteListeTYPE_DOC = "select distinct(TYPE_DOC) "
		RequeteListeTYPE_DOC = RequeteListeTYPE_DOC &amp; "from C_CHARTEDOC as c "
		RequeteListeTYPE_DOC = RequeteListeTYPE_DOC &amp; "INNER JOIN dbo.V_PARAM_COTE as vpc on c.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC "
		RequeteListeTYPE_DOC = RequeteListeTYPE_DOC &amp; "where FILTRE = '1' AND ACTIVITE LIKE '%PAPS%' AND vpc.CODE_ORIGINE = '" &amp; codeOri &amp; "' "
		RequeteListeTYPE_DOC = RequeteListeTYPE_DOC &amp; "order by TYPE_DOC"
		
	else
		RequeteListeTYPE_DOC = "select distinct(TYPE_DOC) from C_CHARTEDOC where FILTRE = '1' AND ACTIVITE LIKE '%PAPS%' order by TYPE_DOC"
	end if
	
	' MAJ 1.1.0 --
	
End function

Function RequeteListeORIGINE()
	dim typeDoc
	
	if ( G("Phase") = "VCD" ) then
		typeDoc = Doc("TYPE_DOC")
	else
		typeDoc = ""
	end if
	
	call TraceDebug("fn","RequeteListeORIGINE","") 
	
	'RequeteListeTYPE_DOC = "select distinct(TYPE_DOC) from C_CHARTEDOC where COTE = 'DAP' AND FILTRE = '1' order by TYPE_DOC"
	
	' Si on a un type de document
	if(typeDoc &lt;&gt; "") then 
		RequeteListeORIGINE = "SELECT vpc.CODE_ORIGINE, LIB_ORIGINE FROM dbo.V_PARAM_COTE vpc" 
		RequeteListeORIGINE = RequeteListeORIGINE &amp; " inner join dbo.V_REF_ORIGINE vro on vro.CODE_ORIGINE = vpc.CODE_ORIGINE"
		RequeteListeORIGINE = RequeteListeORIGINE &amp;" inner join dbo.C_CHARTEDOC cc on vpc.CODE_TYPE_DOC = cc.CODE_TYPE_DOC"
		RequeteListeORIGINE = RequeteListeORIGINE &amp;" WHERE  cc.TYPE_DOC = '" &amp; typeDoc &amp; "' AND cc.ACTIVITE LIKE '%PAPS%'"
	else
		RequeteListeORIGINE = "select CODE_ORIGINE, LIB_ORIGINE from V_REF_ORIGINE order by LIB_ORIGINE"
	end if
	
	
	' MAJ 1.1.0 --
	
End function

Function RequeteListeCOTE()
	dim typeDoc
	dim codeOri
	
	if ( G("Phase") = "VCD" ) then
		typeDoc = Doc("TYPE_DOC")
		codeOri = Doc("CODE_ORIGINE")
	else
		typeDoc = ""
		codeOri = ""
	end if
	
	call TraceDebug("fn","RequeteListeCOTE","") 

	if(typeDoc &lt;&gt; "" and codeOri &lt;&gt; "") then 
		RequeteListeCOTE = "SELECT DISTINCT vrc.LIB_COTE "
		RequeteListeCOTE = RequeteListeCOTE &amp; "FROM dbo.V_REF_COTE as vrc "
		RequeteListeCOTE = RequeteListeCOTE &amp; "INNER JOIN dbo.V_PARAM_COTE as vpc on vrc.CODE_COTE = vpc.CODE_COTE "
		RequeteListeCOTE = RequeteListeCOTE &amp; "inner JOIN dbo.C_CHARTEDOC as c on c.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC "
		RequeteListeCOTE = RequeteListeCOTE &amp; "WHERE c.TYPE_DOC = '" &amp; typeDoc &amp; "' and vpc.CODE_ORIGINE = '" &amp; codeOri &amp; "'"
		
	elseif(typeDoc &lt;&gt; "" and codeOri = "") then		
		RequeteListeCOTE = "SELECT DISTINCT vrc.LIB_COTE "
		RequeteListeCOTE = RequeteListeCOTE &amp; "FROM dbo.V_REF_COTE as vrc "
		RequeteListeCOTE = RequeteListeCOTE &amp; "INNER JOIN dbo.V_PARAM_COTE as vpc on vrc.CODE_COTE = vpc.CODE_COTE "
		RequeteListeCOTE = RequeteListeCOTE &amp; "inner JOIN dbo.C_CHARTEDOC as c on c.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC "
		RequeteListeCOTE = RequeteListeCOTE &amp; "WHERE c.TYPE_DOC = '" &amp; typeDoc &amp;"'"
	elseif(typeDoc = "" and codeOri &lt;&gt; "") then
		RequeteListeCOTE = "SELECT DISTINCT vrc.LIB_COTE "
		RequeteListeCOTE = RequeteListeCOTE &amp; "FROM dbo.V_REF_COTE as vrc "
		RequeteListeCOTE = RequeteListeCOTE &amp; "INNER JOIN dbo.V_PARAM_COTE as vpc on vrc.CODE_COTE = vpc.CODE_COTE "
		RequeteListeCOTE = RequeteListeCOTE &amp; "WHERE vpc.CODE_ORIGINE = '" &amp; codeOri &amp;"'"
	else
		RequeteListeCOTE = "SELECT DISTINCT LIB_COTE FROM dbo.V_REF_COTE"
	end if
	
	
	' MAJ 1.1.0 --
	
End function

Function RequeteListeGESTIONNAIRE_REDAC()
   call TraceDebug("fn","RequeteListeGESTIONNAIRE_REDAC","") 
	RequeteListeGESTIONNAIRE_REDAC = "select DISTINCT(GESTIONNAIRE_REDACTEUR) from V_GESTREDAC_SINI order by GESTIONNAIRE_REDACTEUR"
End function

Function RequeteListeSERVICE()
   call TraceDebug("fn","RequeteListeSERVICE","") 
	'RequeteListeSERVICE = "select SERVICE from V_AFFAIRE order by SERVICE"
	RequeteListeSERVICE = "select DISTINCT(SERVICE) from V_GESTREDAC_SINI order by SERVICE"
End function

Function RequeteListeVisibilite()
	call TraceDebug("fn","RequeteListeVisibilite","") 
	RequeteListeVisibilite = "SELECT REFGEN_ID, REFGEN_LIB FROM V_VISIBILITE"
End function


'=========================================================================================================
' 2 - Fonctions de validation des champs - appelées par ATOME
'=========================================================================================================

'-------------------------------
' 2.1 - Index de niveau PLI
'-------------------------------
' DATE_INTEGRATION
Function DATE_INTEGRATION_Validation()
	call TraceDebug("fn","DATE_INTEGRATION_Validation","") 
	DATE_INTEGRATION_Validation = True

	'msgbox("DATE_INTEGRATION_Validation")

	' ReportIndex : Transfert auto dans chaque doc dans ControleApresLecture
End function

' SENS
Function SENS_Validation()
	call TraceDebug("fn","SENS_Validation","") 
	SENS_Validation = True

	'msgbox("SENS_Validation")
	' ReportIndex : Transfert auto dans chaque doc dans ControleApresLecture
End function

' OPERATEUR
Function OPERATEUR_Validation()
	call TraceDebug("fn","OPERATEUR_Validation","") 
	OPERATEUR_Validation = True
	
	'msgbox("OPERATEUR_Validation")

	' ReportIndex : Transfert auto dans chaque doc dans ControleApresLecture
	if OPERATEUR_Validation then ReportIndexTrans("OPERATEUR")
End function

' NO_DOSSIER
Function NO_DOSSIER_Validation()
   call TraceDebug("fn","NO_DOSSIER_Validation","") 

	' Forced
	if Doc("NO_DOSSIER").Forced = true then
		Doc("NO_DOSSIER") = 0
		Doc("NO_DOSSIER") = ""
		ReportForcedIndexTrans("NO_DOSSIER")
		NO_DOSSIER_Validation = False
		exit function
	end if	
	
	' ajout spécial pour PAPS sur le N° de dossier
	if(len(Doc("NO_DOSSIER")) &gt; 1) then
		if(isOnlyDigit(Mid(Doc("NO_DOSSIER"),1,2)) ) then
			if(ListNO_DOSSIERPluginForced()) then
				NO_DOSSIER_Validation = true
				MsgErreur = ""
			else
				Doc("GESTIONNAIRE_REDAC") = ""
				Doc("CODE_GESTIONNAIRE_REDAC") = ""
				Doc("GROUPE") = ""
				NO_DOSSIER_Validation = false
				MsgErreur = G_msg
				G_msg = ""
				Doc("NO_DOSSIER")=""
			end if
		end if

	elseif(len(Doc("NO_DOSSIER")) = 0) then
		Doc("GESTIONNAIRE_REDAC") = ""
		Doc("CODE_GESTIONNAIRE_REDAC") = ""
		Doc("GROUPE") = ""
		MsgErreur = "Pas de gestionnaire associé"
	end if
	
	ListNO_DOSSIERPluginForced()
	
	' Appel à la BDD
	if ListNO_DOSSIERPlugin() then
		NO_DOSSIER_Validation = true
		MsgErreur = ""
	else
		NO_DOSSIER_Validation = false
		'MsgErreur = "NO_DOSSIER ABSENT DE LA BASE"
		MsgErreur = G_msg
		G_msg = ""
		Doc("NO_DOSSIER")=""
	end if

	' ReportIndex
	if NO_DOSSIER_Validation then 
		ReportIndexTrans("NO_DOSSIER")
		ReportIndexTrans("GESTIONNAIRE_REDAC")
		ReportIndexTrans("CODE_GESTIONNAIRE_REDAC")
	end if

End function


' CODE_DEPARTEMENT
Function CODE_DEPARTEMENT_Validation()
	call TraceDebug("fn","CODE_DEPARTEMENT_Validation","")
	CODE_DEPARTEMENT_Validation = True ' On valide par défaut

	'msgbox("CODE_DEPARTEMENT_Validation")

	' Puis, on reporte l'index dans tout le document
	'if TYPE_ADHERENT_Validation then ReportIndexDocRD("TYPE_ADHERENT")
	if CODE_DEPARTEMENT_Validation then ReportIndexTrans("CODE_DEPARTEMENT")

End function



' NO_ADHERENT
Function NO_ADHERENT_Validation()
	call TraceDebug("fn","NO_ADHERENT_Validation","") 
	
	'msgbox("NO_ADHERENT_Validation")

' Forced
	if Doc("NO_ADHERENT").Forced = true then
		Doc("NO_ADHERENT") = "" ' 2.5.0
		ReportForcedIndexTrans("NO_ADHERENT")
		NO_ADHERENT_Validation = False
		noAdhOk = false
		MsgErreur = ""
		' On demande la vérification des deux champs et impact du status
		'veriFields() 'Remplacé par checkStatus()
		exit function
	end if
	
	if(Doc("NO_ADHERENT") &lt;&gt; "") then
		' Si on a pas renseigné de N° de dossier
		if(Doc("NO_DOSSIER") = "") then
			if(getVueAllAdherent(Doc("NO_ADHERENT"))) then
				NO_ADHERENT_Validation = true
				noAdhOk = true
				G_msg = ""
				MsgErreur = ""
			else
				NO_ADHERENT_Validation = False
				noAdhOk = false
			end if
		else
			NO_ADHERENT_Validation = True
			noAdhOk = true
		end if
		
	else
		NO_ADHERENT_Validation = False
	end if
	
'	if Doc("NO_ADHERENT") &lt;&gt; "" then
'		if(getVueAllAdherent(Doc("NO_ADHERENT"))) then
'			NO_ADHERENT_Validation = true
'			G_msg = ""
'			MsgErreur = ""
'		else
'			NO_ADHERENT_Validation = False
'			noAdhOk = false
'		end if
'	else
'		NO_ADHERENT_Validation = False
'		noAdhOk = false
'		Doc("PROVENANCE") = "" ' On vide la provenance
'		MsgErreur = "SAISIR UN N° OU FORCER SVP"
'	end if

' ------------------------ Mise en commentaire -----------------------------
' Note : Cette régle n'est pas valable dans le cas de PAPS.
'Ce n'est pas du N° d'adhérent que l'on va déduire le gestionnaire rédacteur et les groupes et services, mais du n° de sinistre

'	if(len(Doc("NO_ADHERENT")) &gt; 0) then
'		'On appelle la fonction qui permet de récupérer 
'		'GESTIONNAIRE_REDAC
'		'SERVICE
'		'GROUPE
'		'PROVENANCE
'		
'		' Si la récupération des champs associés à bien fonctionnée 
'		if(RecupProvGestRedacServAndGp()) then 
'			NO_ADHERENT_Validation = True
'			noAdhOk = true
'			MsgErreur = ""
'		else
'			NO_ADHERENT_Validation = False
'			Doc("NO_ADHERENT").Valid = False
'			MsgErreur = "Le N° d'adhérent ne convient pas"
'			Doc("SERVICE") = "PROD" ' On met toujours production comme service
'			noAdhOk = false 
'			
'		end if
'		NO_ADHERENT_Validation = True
'	else
'		' Sinon on vide les champs associés
'		Doc("GESTIONNAIRE_REDAC") = ""
'		Doc("SERVICE") = "SINI" ' On met toujours production comme service
'		Doc("GROUPE") = "" 
'		Doc("PROVENANCE") = ""
'		NO_ADHERENT_Validation = False
'		noAdhOk = false 
'		MsgErreur = ""
'	end if
	
	' On appelle la fonction qui calcule le numéro d'avenant 
	'checkNoAvenant()
	
	' On demande la vérification des deux champs et impact du status
	'veriFields() ' Remplacé par checkStatus()
	checkStatus()
	
	' Si le champ N0_ADHERENT est validé
	'if NO_ADHERENT_Validation then 	ReportIndexDocRD("NO_ADHERENT")
	if NO_ADHERENT_Validation then 	ReportIndexTrans("NO_ADHERENT")
	
End function


' TYPE_ADHERENT
Function TYPE_ADHERENT_Validation()
	call TraceDebug("fn","TYPE_ADHERENT_Validation","")
	TYPE_ADHERENT_Validation = True ' On valide par défaut

	'msgbox("TYPE_ADHERENT_Validation")

	' Puis, on reporte l'index dans tout le document
	'if TYPE_ADHERENT_Validation then ReportIndexDocRD("TYPE_ADHERENT")
	if TYPE_ADHERENT_Validation then ReportIndexTrans("TYPE_ADHERENT")

End Function



' REF_TIERS
Function REF_TIERS_Validation()
	call TraceDebug("fn","REF_TIERS_Validation","")
	REF_TIERS_Validation = True ' On valide par défaut

	'msgbox("REF_TIERS_Validation")

	' Puis, on reporte l'index dans tout le document
	if REF_TIERS_Validation then ReportIndexDocRD("REF_TIERS")

End function



' PROVENANCE
Function PROVENANCE_Validation()
	call TraceDebug("fn","PROVENANCE_Validation","") 
	'PROVENANCE_Validation = True
	
	'msgbox("PROVENANCE_Validation")

	' Forced
	if Doc("PROVENANCE").Forced = true then
		' Modification du 11/05/2012
		'ReportForcedIndexTrans("PROVENANCE")
		ReportForcedIndexDocRD("PROVENANCE")
		PROVENANCE_Validation = False
		exit function
	end if
	
	
	if(Doc("PROVENANCE") &lt;&gt; "") then
		MsgErreur = ""
		PROVENANCE_Validation = True
	else
		MsgErreur = "Merci de saisir une valeur dans la liste"
		PROVENANCE_Validation = False
	end if

'	ReportIndex
	'if PROVENANCE_Validation then ReportIndexDocRD("PROVENANCE")
	if PROVENANCE_Validation then ReportIndexTrans("PROVENANCE")
'
'	'call TraceDebug("data","PROVENANCE_Validation","PROVENANCE=" &amp; Doc("PROVENANCE")) 
End function


' SERVICE
Function SERVICE_Validation()
	call TraceDebug("fn","SERVICE_Validation","") 
	
	'msgbox("SERVICE_Validation")

	' Forced
	if Doc("SERVICE").Forced = true then
		ReportForcedIndexDocRD("SERVICE")
		SERVICE_Validation = False
		exit function
	end if
	
	
	SERVICE_Validation = True

'	' ReportIndex sur le document
	'if SERVICE_Validation then ReportIndexDocRD("SERVICE")
	if SERVICE_Validation then ReportIndexTrans("SERVICE")
End function


' GROUPE
Function GROUPE_Validation()
	call TraceDebug("fn","GROUPE_Validation","") 
	GROUPE_Validation = false
	
	'msgbox("GROUPE_Validation")

	' Forced
	if Doc("GROUPE").Forced = true then
		'ReportForcedIndexTrans("GROUPE")
		ReportForcedIndexDocRD("GROUPE")
		GROUPE_Validation = False
		exit function
	end if
	
	' Si le groupe est bon
	if(Doc("GROUPE") = "1" or Doc("GROUPE") = "2" or Doc("GROUPE") = "3" or Doc("GROUPE") = "4" or Doc("GROUPE") = "5" or Doc("GROUPE") = "6" or Doc("GROUPE") = "7" or Doc("GROUPE") = "8" or Doc("GROUPE") = "9") or Doc("GROUPE") = "10" or Doc("GROUPE") = "11" or Doc("GROUPE") = "12" or Doc("GROUPE") = "13" or Doc("GROUPE") = "14" or Doc("GROUPE") = "15" or Doc("GROUPE") = "17" or Doc("GROUPE") = "18" or Doc("GROUPE") = "20" or Doc("GROUPE") = "21" or Doc("GROUPE") = "22" or Doc("GROUPE") = "24" then
		GROUPE_Validation = true
		' Si le numéro de dossier est vide
		if(Doc("NO_DOSSIER").valid = false ) then
			Doc("GESTIONNAIRE_REDAC") = "" 
			Doc("GESTIONNAIRE_REDAC").forced = true
			Doc("CODE_GESTIONNAIRE_REDAC") = ""
			Doc("CODE_GESTIONNAIRE_REDAC").forced = true
		end if
		MsgErreur = ""
	else
		GROUPE_Validation = false
		MsgErreur = "Le numéro de groupe ne correspond pas à un groupe PAPS"
	end if
	
'	' ReportIndex sur le pli
	if GROUPE_Validation then ReportIndexTrans("GROUPE")
	
	checkStatus()
	
End function

'-------------------------------
' 2.2 - Index de niveau DOCUMENT
'-------------------------------

' DATE_DOCUMENT
' OK VALIDE
Function DATE_DOCUMENT_Validation()	
	call TraceDebug("fn","DATE_DOCUMENT_Validation","") 
	
	Dim DateEtab
	Dim FormatDateEtab
	Dim DateJour
	Dim JJ
	Dim MM
	Dim AAAA
	Dim Val
	
	'msgbox("DATE_DOCUMENT_Validation")

	' Vérif Date si forcée
	if Doc("DATE_DOCUMENT").Forced = true then
		if ( len(Doc("DATE_DOCUMENT")) = 8 ) or ( len(Doc("DATE_DOCUMENT")) = 0 ) then
			if ( len(Doc("DATE_DOCUMENT")) = 8 ) then
				DateEtab = DOC("DATE_DOCUMENT")
				JJ	= Mid(DateEtab,1,2)
				MM	= Mid(DateEtab,3,2)
				AAAA = Mid(DateEtab,5,4)
				if ( MyIsDate(JJ, MM, AAAA) ) then
					ReportForcedIndexDocRD("DATE_DOCUMENT")
				else
					Doc("DATE_DOCUMENT").Forced = false
					MsgErreur = "DATE NON VALIDE ! FORMAT : JJMMAAAA"				
				end if
			else
				ReportForcedIndexDocRD("DATE_DOCUMENT")		
			end if
		else
			Doc("DATE_DOCUMENT").Forced = false
			MsgErreur = "DATE DOIT ETRE JJMMAAAA !"				
		end if
		DATE_DOCUMENT_Validation = False
		exit function
	end if		
	
	' Vérif date si non forcée
	DateEtab = DOC("DATE_DOCUMENT")
	if ( len(DateEtab) = 8 ) then	
		JJ	= Mid(DateEtab,1,2)
		MM	= Mid(DateEtab,3,2)
		AAAA	= Mid(DateEtab,5,4)
	else
		if ( len(DateEtab) = 4 ) then
			DateJour = date   ' DateJour contient la date système actuelle.
			JJ	= Mid(DateEtab,1,2)
			MM	= Mid(DateEtab,3,2)
			AAAA= Year(DateJour)
			FormatDateEtab = DateSerial(AAAA, MM, JJ)
			if (DateDiff("d", date, FormatDateEtab) &gt; 0) then
				AAAA = AAAA - 1
			end if
			DateEtab = DateEtab &amp; AAAA
			DOC("DATE_DOCUMENT") = DateEtab
		else
			MsgErreur = "DATE DOIT JJMMAAAA OU JJMM !"	
			DATE_DOCUMENT_Validation = false
		  exit function	
		end if
	end if
	
	FormatDateEtab = DateSerial(AAAA, MM, JJ)
	
	DateJour = date   ' DateJour contient la date système actuelle.
	
	if (DateDiff("d", date, FormatDateEtab) &gt; 0) then
	      DATE_DOCUMENT_Validation = false
	      MsgErreur = "DATE DOC &gt; A LA DATE DU JOUR !"
      exit function
	end if
	
	if AAAA &lt; 1850 then
	      DATE_DOCUMENT_Validation = false
	      MsgErreur = "ANNEE DOC &lt; 1850 !"
      exit function
	end if
	
	if MyIsDate(JJ,MM,AAAA) then 
		DATE_DOCUMENT_Validation = true
		MsgErreur = ""
	else
	    MsgErreur = "DATE NON VALIDE"	
		DATE_DOCUMENT_Validation = false
	end if
	
	if DATE_DOCUMENT_Validation then ReportIndexDocRD("DATE_DOCUMENT")
	checkStatus()
End function


' GESTIONNAIRE_REDAC
Function GESTIONNAIRE_REDAC_Validation()
	call TraceDebug("fn","GESTIONNAIRE_REDAC_Validation","") 
	'GESTIONNAIRE_REDAC_Validation = True
	
	'msgbox("GESTIONNAIRE_REDAC_Validation")

	' forced
	if Doc("GESTIONNAIRE_REDAC").Forced = true then
		ReportForcedIndexDocRD("GESTIONNAIRE_REDAC")
		GESTIONNAIRE_REDAC_Validation = false
		Doc("CODE_GESTIONNAIRE_REDAC").Forced = true
		exit function
	end if
	
	
	' forced
'	if Doc("GESTIONNAIRE_REDAC").Forced = true then
'		' Si le gestionnaire redac est vide, on vide les champs associés
'		if(Doc("GESTIONNAIRE_REDAC") = "")then
'			Doc("GROUPE").Value = ""
'			Doc("SERVICE").Value = ""
'			Doc("CODE_GESTIONNAIRE_REDAC").Value = ""
'		end if
'		
'		' On force les champs correspondants
'		Doc("GROUPE").Forced = true
'		Doc("SERVICE").Forced = true
'		Doc("CODE_GESTIONNAIRE_REDAC").Forced = true
'		
'		'Si on a une valeur de gestionnaire rédacteur
'		if( len(Doc("GESTIONNAIRE_REDAC")) &gt; 0 )then
'		
'			'Si le plugin a rapatrié les informations du gestionnaire
'			if (ListGESTIONNAIRE_REDACForced_Plugin())  then
'				'getInfoRedac() ' On récupére les informations du gestionnaire
'				ReportForcedIndexDocRD("GESTIONNAIRE_REDAC")
'				ReportForcedIndexDocRD("GROUPE")
'				ReportForcedIndexDocRD("SERVICE")
'				ReportForcedIndexDocRD("CODE_GESTIONNAIRE_REDAC")
'	
'				GESTIONNAIRE_REDAC_Validation = True
'				exit function
'			else
'				alertInVcd("Code '"&amp; Doc("GESTIONNAIRE_REDAC") &amp;"' gestionnaire introuvable")
'				'Doc("GESTIONNAIRE_REDAC").Forced = false
'				GESTIONNAIRE_REDAC_Validation = False
'				GESTIONNAIRE_REDAC_Validation = ""
'				exit function
'			end if
'		else
'			GESTIONNAIRE_REDAC_Validation = false
'			exit function
'		end if
'		
'		exit function
'	end if	


	' valid
	
	' Si le numéro de dossier n'existe pas, ou n'est pas validé
	if(len(doc("NO_DOSSIER"))  = 0 or doc("NO_DOSSIER").valid = false) then
		GESTIONNAIRE_REDAC_Validation = False
		MsgErreur = "Sinistre + gestionnaire redac ou groupe + force"
		Doc("CODE_GESTIONNAIRE_REDAC") = ""
		Doc("GESTIONNAIRE_REDAC") = ""
	
	else
		if len(doc("GESTIONNAIRE_REDAC")) &gt; 0 then
			GESTIONNAIRE_REDAC_Validation = True
			getInfoRedac() ' On récupére les informations du gestionnaire
			MsgErreur = ""
		else
			GESTIONNAIRE_REDAC_Validation = False
			MsgErreur = "Veuillez sélectionner un rédacteur"
			Doc("CODE_GESTIONNAIRE_REDAC") = ""
		end if
	
	end if
	

	' ReportIndex
	if GESTIONNAIRE_REDAC_Validation then ReportIndexDocRD("GESTIONNAIRE_REDAC")
	'if GESTIONNAIRE_REDAC_Validation then ReportIndexTrans("GESTIONNAIRE_REDAC")

	checkStatus()

End function


' CODE_GESTIONNAIRE_REDAC
Function CODE_GESTIONNAIRE_REDAC_Validation()
	call TraceDebug("fn","CODE_GESTIONNAIRE_REDAC_Validation","") 
	
	'msgbox("CODE_GESTIONNAIRE_REDAC_Validation")

	' Forced
	if Doc("CODE_GESTIONNAIRE_REDAC").Forced = true then
	
		' Si le code gestionnaire redac est forcé et vide, on vide les champs associés
		if(Doc("CODE_GESTIONNAIRE_REDAC") = "")then
			'Doc("GROUPE").Value = 0
			Doc("SERVICE").Value = ""
			Doc("GESTIONNAIRE_REDAC").Value = ""
			CODE_GESTIONNAIRE_REDAC_Validation = False
			ReportForcedIndexDocRD("CODE_GESTIONNAIRE_REDAC")
			exit function
		end if
		
		' On force les champs correspondants
		Doc("GROUPE").Forced = true
		Doc("SERVICE").Forced = true
		Doc("GESTIONNAIRE_REDAC").Forced = true
		
		
		
		
		if(len(doc("NO_DOSSIER")) = 0 or doc("NO_DOSSIER").valid = false) then
			CODE_GESTIONNAIRE_REDAC_Validation = False
			MsgErreur = "Choisir N° sinistre, pour attribuer un code (ou forcer)"
			Doc("CODE_GESTIONNAIRE_REDAC") = ""
			Doc("GESTIONNAIRE_REDAC") = ""
			Doc("GROUPE") = ""
	
		else
			'Si on a une valeur de gestionnaire rédacteur
			if( len(Doc("CODE_GESTIONNAIRE_REDAC")) &gt; 0 )then
			
				'Si le plugin a rapatrié les informations du gestionnaire
				if (ListGESTIONNAIRE_REDACForced_Plugin())  then
					'getInfoRedac() ' On récupére les informations du gestionnaire
					ReportForcedIndexDocRD("GESTIONNAIRE_REDAC")
					ReportForcedIndexDocRD("GROUPE")
					ReportForcedIndexDocRD("SERVICE")
					ReportForcedIndexDocRD("CODE_GESTIONNAIRE_REDAC")
		
					CODE_GESTIONNAIRE_REDAC_Validation = False
					exit function
				else
					alertInVcd("Code '"&amp; Doc("CODE_GESTIONNAIRE_REDAC") &amp;"' gestionnaire introuvable")
					Doc("CODE_GESTIONNAIRE_REDAC").Forced = false ' On déforce le champs
					' On vide les champs associés au code gestionnaire rédacteur
					Doc("GROUPE").Value = 0
					Doc("SERVICE").Value = ""
					Doc("GESTIONNAIRE_REDAC").Value = ""
					CODE_GESTIONNAIRE_REDAC_Validation = False
					exit function
				end if
			else
				CODE_GESTIONNAIRE_REDAC_Validation = false
				exit function
			end if
		
		end if
	end if

	' Dans le cas ou il n'est pas forcé
	if( len(Doc("CODE_GESTIONNAIRE_REDAC")) &gt; 0) then
		if(Doc("NO_DOSSIER").Valid = false ) then
			CODE_GESTIONNAIRE_REDAC_Validation = false
			Doc("CODE_GESTIONNAIRE_REDAC").Value = ""
			Doc("GESTIONNAIRE_REDAC").Value = ""
			Doc("SERVICE").Value = ""
			MsgErreur = "Saisir un numéro de sinistre ou forcer"
		else
			
			' Si les informations du gestionnaire ont été trouvées
			if(getInfoRedacByCode()) then
				MsgErreur = ""
				CODE_GESTIONNAIRE_REDAC_Validation = True
			else
				MsgErreur = "Le trigramme semble introuvable"
				CODE_GESTIONNAIRE_REDAC_Validation = false
			end if
		end if
		
		
	else
		CODE_GESTIONNAIRE_REDAC_Validation = false
	end if
	
'	if CODE_GESTIONNAIRE_REDAC_Validation then ReportIndexTrans("CODE_GESTIONNAIRE_REDAC")
	if CODE_GESTIONNAIRE_REDAC_Validation then ReportIndexDocRD("CODE_GESTIONNAIRE_REDAC")
	
End function




' FAMILLE
Function FAMILLE_Validation()
	call TraceDebug("fn","FAMILLE_Validation","")
	
	'msgbox("FAMILLE_Validation")

'	if(Len(Doc("FAMILLE")) = 0 ) then
'		FAMILLE_Validation = False
'	else
'		FAMILLE_Validation = True
'	end if
	
	dim partieGaucheWex, PosTiret, taillePartieGauche, TailleCodePiece
	
	PosTiret = Instr(DOC("FormNameRecto"), "_") 
	TailleCodePiece = Len(DOC("FormNameRecto")) - PosTiret
	
	' On calcule la taille de la partie droite du formulaire WEX
	'taillePartieGauche = (Len(DOC("FormNameRecto")) - TailleCodePiece) + 1
	taillePartieGauche = 5
	
	' On récupére la partie droite du formulaire WEX
	if ( len(DOC("FormNameRecto")) &gt; PosTiret ) then
		Doc("UserField010") = Mid(DOC("FormNameRecto"), PosTiret+1, TailleCodePiece)   ' Code Pièce
		partieGaucheWex = Mid(DOC("FormNameRecto"), 1, taillePartieGauche) ' Partie gauche du Formulaire Wex
	else
		Doc("UserField010") = ""
		partieGaucheWex = ""
	end if

	'alertInVcd("Form : " &amp;amp; partieGaucheWex)

'	Doc("FAMILLE") = "CONTRAT RCE"
'	FAMILLE_Validation = True


	' Si on est en phase de vidéoCodage et que le champs est vide, alors on propose la liste
	if G("Phase") = "VCD" then
		if(Doc("FAMILLE") = "") then 
			FAMILLE_Validation = False				
		else
			FAMILLE_Validation = true
		end if
	else
'			Doc("FAMILLE") = "CONTRAT RCE"
'			FAMILLE_Validation = True
'		elseif(partieGaucheWex = "CTRCT") then
'			Doc("FAMILLE") = "CONTRAT RCT"
'			FAMILLE_Validation = True
'		elseif(partieGaucheWex = "CBAS1") then
'			Doc("FAMILLE") = "CONTRAT BASE"
'			FAMILLE_Validation = True
'		elseif(partieGaucheWex = "CTPJP") then
'			Doc("FAMILLE") = "CONTRAT PJP"
'			FAMILLE_Validation = True
'		elseif(partieGaucheWex = "CTSPS") then
'			Doc("FAMILLE") = "CONTRAT SPS"
'			FAMILLE_Validation = True
'		elseif(partieGaucheWex = "CTINT") then
'			Doc("FAMILLE") = "CONTRAT INTERNATIONAL"
'			FAMILLE_Validation = True
'		else
		Doc("FAMILLE") = "DOCUMENTS PAPS"
		FAMILLE_Validation = True
		'end if
	
	end if
		
	
	'FAMILLE_Validation = True
	'report
	if FAMILLE_Validation then ReportIndexDocRD("FAMILLE")
	
End function




' TYPE_DOC
Function TYPE_DOC_Validation()
	call TraceDebug("fn","TYPE_DOC_Validation","") 
	
	'msgbox("TYPE_DOC_Validation")

	NO_DOSSIER_Validation()

	' forced
	if Doc("TYPE_DOC").Forced = true then
		Doc("TYPE_DOC").Value = ""		
		ReportForcedIndexDocRD("TYPE_DOC")
		TYPE_DOC_Validation = False
		exit function
	end if	

	' valid
	if len(doc("TYPE_DOC")) &gt; 0 then
		TYPE_DOC_Validation = True
		MsgErreur = ""
	else
		TYPE_DOC_Validation = False
		'exit function ' 2.6.3
	end if

	' Récup des index liés au TypeDoc
	if RecupCHARTEDOC(Doc("TYPE_DOC")) then
		TYPE_DOC_Validation = true
		MsgErreur = ""
	else
		TYPE_DOC_Validation = false
		MsgErreur = G_msg
		G_msg = ""
	end if

	' report
	if TYPE_DOC_Validation then
		'getTheCote() ' On essaye de déduire la cote
		ReportIndexDocRD("TYPE_DOC")
		ReportIndexDocRD("FAMILLE")
		ReportIndexDocRD("COTE")
		ReportIndexDocRD("CONSERV_ORIGINAL")
		ReportIndexDocRD("VALEUR_JURIDIQUE")
		'G_msg = "" 'On vide le message d'erreur - Ceci est fait dans getTheCote
	end if

	'PAPSV11 On affect le code gestionnaire du gestionnairte de numerisation si le type doc est bordereau de reglement
	if Doc("TYPE_DOC")="BORDEREAU DE REGLEMENT" then
		Doc("CODE_GESTIONNAIRE_REDAC") = Doc("CodeClient")
		Doc("GESTIONNAIRE_REDAC").valid = true
		Doc("CODE_GESTIONNAIRE_REDAC").valid  = true
		ReportIndexDocRD("CODE_GESTIONNAIRE_REDAC")
		ReportIndexDocRD("GESTIONNAIRE_REDAC")
	end if

	'P9/C9  On doit faire appel a la fonction de recherche du code gestionnaire redacteur si type_document = 'NOTE HONORAIRES' et si la CODE_ORIGINE est differente de ADHER
	if Doc("TYPE_DOC")="NOTE HONORAIRES" then
		if RechercheCodeRedacNoteHonoraire() then
			TYPE_DOC_Validation = true
			Doc("GESTIONNAIRE_REDAC").valid = true
			Doc("CODE_GESTIONNAIRE_REDAC").valid  = true
			ReportIndexDocRD("CODE_GESTIONNAIRE_REDAC")
			ReportIndexDocRD("GESTIONNAIRE_REDAC")
			MsgErreur = ""
		else
			TYPE_DOC_Validation = false
			MsgErreur = G_msg
			G_msg = ""
		end if
	end if
	
	checkStatus()
	
End function

' COTE
Function COTE_Validation()
    call TraceDebug("fn","COTE_Validation","") 
	
	'msgbox("COTE_Validation")

	COTE_Validation = True

	' report
	if COTE_Validation then ReportIndexDocRD("COTE")
End function

' LIB_DOC
Function LIB_DOC_Validation()
    call TraceDebug("fn","LIB_DOC_Validation","") 
    
   	'msgbox("LIB_DOC_Validation")

	' forced
	if Doc("LIB_DOC").Forced = true then
		Doc("LIB_DOC").Value = Doc("TYPE_DOC") &amp; " " &amp; libOrigine
		ReportForcedIndexDocRD("LIB_DOC")
		LIB_DOC_Validation = False
		MsgErreur = ""
		exit function
	end if	
    
	if(Doc("LIB_DOC") &lt;&gt; "") then
		LIB_DOC_Validation = true
		MsgErreur = ""	
	else
		LIB_DOC_Validation = false
		MsgErreur = "Merci de saisir un libellé ou forcer"	
	end if
'	
' report
	if LIB_DOC_Validation then ReportIndexDocRD("LIB_DOC")
End function

' OBSERVATION
Function OBSERVATION_Validation()
    call TraceDebug("fn","OBSERVATION_Validation","") 
	OBSERVATION_Validation = True

	'msgbox("OBSERVATION_Validation")

' report
	if OBSERVATION_Validation then ReportIndexDocRD("OBSERVATION")
End function


' DATE_NUM
Function DATE_NUM_Validation()
	call TraceDebug("fn","DATE_NUM_Validation","") 
	DATE_NUM_Validation = True
	
	'msgbox("DATE_NUM_Validation")
	
	if DATE_NUM_Validation then ReportIndexDocRD("DATE_NUM")
End function


 'MODE
Function MODE_Validation()
   call TraceDebug("fn","MODE_Validation","") 
   MODE_Validation = True

	'msgbox("MODE_Validation")

	if(Doc("MODE") = "COURRIER") then
		Doc("PRESENCE_AR") = 0
	elseif (Doc("MODE") = "COURRIER AR") then
		Doc("PRESENCE_AR") = 1
	end if


'	' report
	'if MODE_Validation then ReportIndexDocRD("MODE")
	if MODE_Validation then ReportIndexTrans("MODE")
	
	checkStatus()
	
End function

' STATUS
Function STATUS_Validation()
   call TraceDebug("fn","STATUS_Validation","") 

	'msgbox("STATUS_Validation")

   STATUS_Validation = True

	if STATUS_Validation then ReportIndexDocRD("STATUS")
End function

Function PRESENCE_AR_Validation()
	call TraceDebug("fn","PRESENCE_AR_Validation","") 
	
	'msgbox("PRESENCE_AR_Validation")

	if(Doc("PRESENCE_AR") &lt;&gt; "" ) then
		
		if(Doc("PRESENCE_AR") = 1) then
			Doc("MODE") = "COURRIER AR"
			PRESENCE_AR_Validation = True
		elseif(Doc("PRESENCE_AR") = 0) then
			Doc("MODE") = "COURRIER"
			PRESENCE_AR_Validation = True
		end if
	else
		PRESENCE_AR_Validation = False
	end if

	if PRESENCE_AR_Validation then ReportIndexTrans("PRESENCE_AR")
	checkStatus()
End Function


'REF_ARCHIVE
Function REF_ARCHIVE_Validation()
	call TraceDebug("fn","REF_ARCHIVE_Validation","") 
	'msgbox("REF_ARCHIVE_Validation")
	REF_ARCHIVE_Validation = True
	if REF_ARCHIVE_Validation then ReportIndexDocRD("REF_ARCHIVE")

End Function

' VISIBILITE
Function VISIBILITE_EXTERNE_Validation()
	call TraceDebug("fn","VISIBILITE_EXTERNE_Validation","") 
	'Doc("VISIBILITE_EXTERNE") = 0
	'msgbox("VISIBILITE_EXTERNE_Validation")
	VISIBILITE_EXTERNE_Validation = True
	if VISIBILITE_EXTERNE_Validation then ReportIndexDocRD("VISIBILITE_EXTERNE")
End Function

' STATUS_INDEXATION
Function STATUS_INDEXATION_Validation()
   call TraceDebug("fn","STATUS_INDEXATION_Validation","") 
   STATUS_INDEXATION_Validation = True

	'msgbox("STATUS_INDEXATION_Validation")
	' ICI ajouter un appel vers une fonction qui check que tous un ensemble de champs sont saisis
	
'	' report d'index dans le document
	if STATUS_INDEXATION_Validation then ReportIndexDocRD("STATUS_INDEXATION")
End function



'CODE_ORIGINE
Function CODE_ORIGINE_Validation()
	call TraceDebug("fn","CODE_ORIGINE_Validation","") 
	
	'msgbox("CODE_ORIGINE_Validation")

	NO_DOSSIER_Validation()

	' forced
	if Doc("CODE_ORIGINE").Forced = true then
		Doc("CODE_ORIGINE").Value = ""
		
		ReportForcedIndexDocRD("CODE_ORIGINE")
		CODE_ORIGINE_Validation = False
		exit function
	end if	
	
	if(Doc("CODE_ORIGINE") = "") then
		CODE_ORIGINE_Validation = False
	else
		CODE_ORIGINE_Validation = True
	end if 
	
	'PAPSV11 On affect le code gestionnaire du gestionnairte de numerisation si le type doc est bordereau de reglement
	if Doc("TYPE_DOC")="BORDEREAU DE REGLEMENT" then
		Doc("CODE_GESTIONNAIRE_REDAC") = Doc("CodeClient")
		Doc("GESTIONNAIRE_REDAC").valid = true
		Doc("CODE_GESTIONNAIRE_REDAC").valid  = true
		ReportIndexDocRD("CODE_GESTIONNAIRE_REDAC")
		ReportIndexDocRD("GESTIONNAIRE_REDAC")
	end if
	
	'P9/C9  On doit faire appel a la fonction de recherche du code gestionnaire redacteur si type_document = 'NOTE HONORAIRES' et si la CODE_ORIGINE est differente de ADHER
	if Doc("TYPE_DOC")="NOTE HONORAIRES" then
		if RechercheCodeRedacNoteHonoraire() then
			CODE_ORIGINE_Validation = true
			Doc("GESTIONNAIRE_REDAC").valid = true
			Doc("CODE_GESTIONNAIRE_REDAC").valid  = true
			ReportIndexDocRD("CODE_GESTIONNAIRE_REDAC")
			ReportIndexDocRD("GESTIONNAIRE_REDAC")
			MsgErreur = ""
		else
			CODE_ORIGINE_Validation = false
			MsgErreur = G_msg
			G_msg = ""
		end if
	end if

	if CODE_ORIGINE_Validation then 
		getTheLibOrigine() ' On stocke le libellé correspondant
		ReportIndexDocRD("CODE_ORIGINE")
		getTheCote() ' On essaye de déduire la cote
	end if
	
	checkStatus()
	
End Function 





'=========================================================================================================
' 3.2 - Fonctions de récup de données - appelées par la validation des champs
'=========================================================================================================
Function RecupCHARTEDOC(TYPE_DOC)
	' Pour une TYPE_DOC : Renvoi Famille, persa notifier, Conserv ori, etc...
	call TraceDebug("fn","RecupCHARTEDOC",TYPE_DOC) 
	Dim Nombre
	dim Recordset
	dim delai1
	dim delai2
	dim date_delai1
	dim date_delai2
	dim LocalTYPE_DOC
	
	dim localFamille
	
	G_msg = ""
	Nombre = 0
	RecupCHARTEDOC = false
	
	' Préparation du champ TYPE_DOC
	LocalTYPE_DOC = replace(TYPE_DOC, "'", "''")
	
	' Préparation du champ FAMILLE
	localFamille = Doc("FAMILLE")
	localFamille = replace(localFamille, "'", "''")


	' On compte le nombre de résultats C_CHARTEDOC
	'If DB.RecordCreate("CountCHARTEDOC","Select Count(*) as Nombre from C_CHARTEDOC where TYPE_DOC = '" &amp; LocalTYPE_DOC &amp; "'") then
	If DB.RecordCreate("CountCHARTEDOC","Select Count(*) as Nombre from C_CHARTEDOC where TYPE_DOC = '" &amp; LocalTYPE_DOC &amp; "' AND FAMILLE = '"&amp; localFamille &amp;"' AND ACTIVITE LIKE '%PAPS%'") then
		Nombre = DB.RecordValue("CountCHARTEDOC","Nombre")
	end if
	
	' Récup infos
	if Nombre = 1 then
		Recordset = "CHARTEDOC"
		If DB.RecordCreate(Recordset,"Select * from C_CHARTEDOC where TYPE_DOC = '" &amp; LocalTYPE_DOC &amp; "' AND FAMILLE = '"&amp; localFamille &amp;"'  AND ACTIVITE LIKE '%PAPS%'") then
			'DOC("FAMILLE") = DB.RecordValue(Recordset,"FAMILLE")
			DOC("COTE") = DB.RecordValue(Recordset,"COTE")
			'DOC("PER_SRV_A_NOTIFIER") = DB.RecordValue(Recordset,"PER_SRV_A_NOTIFIER")
			DOC("CONSERV_ORIGINAL") = DB.RecordValue(Recordset,"CONSERV_ORIGINAL")
			'DOC("PHASE") = DB.RecordValue(Recordset,"PHASE")
			DOC("VALEUR_JURIDIQUE") = DB.RecordValue(Recordset,"VALEUR_JURIDIQUE")
			'DOC("LECTURE_PAPIER") = DB.RecordValue(Recordset,"LECTURE_PAPIER")
			'DOC("CONSERVATION_TEMP") = DB.RecordValue(Recordset,"CONSERVATION_TEMP")
			'DOC("ARCHIVAGE_LONG") = DB.RecordValue(Recordset,"ARCHIVAGE_LONG")
			'DOC("TYPE_CONTACT") = DB.RecordValue(Recordset,"TYPE_CONTACT")
			'DOC("PRIORITE") = DB.RecordValue(Recordset,"PRIORITE")
			
			' On récupére les délais de rétention des types de documents correspondants
			'delai1 = DB.RecordValue(Recordset,"DELAI1")
			'delai2 = DB.RecordValue(Recordset,"DELAI2")
			
			'date_delai1 = DateAdd("d", delai1, Doc("DATE_NUM"))			
			'date_delai2 = DateAdd("d", delai2, Doc("DATE_NUM"))	

			'DOC("DATE_NEXT_ACTION1") = date_delai1 ' 2.5.0
			'DOC("DATE_NEXT_ACTION2") = date_delai2 ' 2.5.0


			'DOC("NEXT_ACTION1") = DB.RecordValue(Recordset,"NEXT_ACTION1") ' 2.5.0
			'DOC("NEXT_ACTION2") = DB.RecordValue(Recordset,"NEXT_ACTION2") ' 2.5.0
			RecupCHARTEDOC = true
			G_msg = ""
		end if
	elseif Nombre = 0 then
		G_msg = "Aucun résultat TYPE_DOC correspondant"
	else
		G_msg = "CHARTEDOC : Doublons(1) de TYPE_DOC"
	end if
		
End function


' Fonction RecupProvGestRedacServAndGp
' Détails : Cette fonction récupére automatiquement la provenance, le gestionnaire rédacteur, le service &amp; le groupe
' Utilisé : Dans le cadre de l'application CPDAP.xml
Function RecupProvGestRedacServAndGp()
	call TraceDebug("fn","RecupProvGestRedacServAndGp","")
	
	dim noSini
	dim reqSql
	dim libGestRedacInView
	dim libGestRedacRedo
	dim arrayLibGestRedac
	dim Nombre
	dim Recordset
	
	noSini = Doc("NO_DOSSIER") ' On récupére le numéro d'adhérent
	
	' Si il y a un numéro de dossier
	if (len(noSini) &gt; 0) then
		
		' Count
		'If DB.RecordCreate("countAdh","Select Count(*) as Nombre from V_AFFAIRE where NO_CLIENT = " &amp; noAdh &amp;"") then
		If DB.RecordCreate("countSin","Select Count(*) as Nombre from V_AFFAIRE_PAPS where WHERE Code_societe + Annee_survenance + Code_Population + Numero_Sinistre + Cle_sinistre = '" &amp; noSini &amp; "'") then
			Nombre = DB.RecordValue("countSin","Nombre")
		end if
		
		if (Nombre &gt; 0) then 
			
			Recordset = "PROVGEST"
			MsgErreur = ""
			'reqSql = "SELECT Gestionnaire_redacteur FROM V_AFFAIRE"
			reqSql = "SELECT va.Gestionnaire_Redacteur, va.Service, va.Groupe, aa.FULL_NAME"
			reqSql = reqSql &amp; " " &amp; "FROM V_AFFAIRE_PAPS as va"
			reqSql = reqSql &amp; " " &amp; "inner join V_ALL_ADHERENT as aa on va.numero_Adherent = aa.NO_CLIENT"
			reqSql = reqSql &amp; " " &amp; "WHERE Code_societe + Annee_survenance + Code_Population + cast(Numero_Sinistre as Varchar(8)) + Cle_sinistre = '" &amp; noSini &amp; "'"
	
			if DB.RecordCreate(Recordset,reqSql) then 
				DOC("SERVICE") = DB.RecordValue(Recordset,"va.Service")
				DOC("GROUPE") = DB.RecordValue(Recordset,"va.Groupe")
				'DOC("PROVENANCE") = DB.RecordValue(Recordset,"aa.FULL_NAME")
				
				
				'Modification du 03/2012 - On ne stocke plus
				'DOC("PROVENANCE") = replace( DB.RecordValue(Recordset,"aa.FULL_NAME"),"'"," ")
			
				' Le libellé du gestionnaire rédacteur n'est pas formatté comme souhaité dans IRIS
				' On le récupére donc, on le découpe, on le TOUPPER &amp; on le remet dans l'ordre [NOM] [PRENOM]
''				libGestRedacInView = DB.RecordValue(Recordset,"LIB_GEST")
''				arrayLibGestRedac = Split(libGestRedacInView," ",-1,1)
''				' On remet dans l'ordre NOM Prénom
''				libGestRedacRedo = arrayLibGestRedac(1) &amp; " " &amp; arrayLibGestRedac(0)
''				' On remet en Majuscule pour avoir NOM PRENOM
''				DOC("GESTIONNAIRE_REDAC") = ucase(libGestRedacRedo)
				DOC("GESTIONNAIRE_REDAC") = DB.RecordValue(Recordset,"va.Gestionnaire_Redacteur")
				RecupProvGestRedacServAndGp = True
				
				' On met à true la variable globale, qui dit si on a trouvé ou non l'adhérent
				siniFounded = true
				
			end if
		else
			MsgErreur = "Sinistre introuvable en BDD"
			
			' On met à true la variable globale, qui dit si on a trouvé ou non l'adhérent
			siniFounded = false
			
			'alertInVcd("Il n'y a pas d'adhérent correspond au numéro "&amp;noAdh)
			RecupProvGestRedacServAndGp = False ' On retourne faux (non trouvé)
		end if
		
	end if


End function

' Fonction veriFields
' Détails : Cette fonction vérifie la conformité des champs NO_ADHERENT &amp; DATE_DOCUMENT et envoi en corbeille si (un des 2) pas renseigné
' Utilisé : Dans le cadre de l'application CPDAP.xml par les champs "anneeDAP" &amp; "NO_ADHERENT"
function veriFields()
	call TraceDebug("fn","veriFields","")
	
	' Si un des champs DATE_DOCUMENT ou NO_ADHERENT est forcé ou l'adhérent n'est pas trouvé
	'if( (DOC("anneeDAP").Forced or DOC("NO_ADHERENT").Forced) or (DOC("anneeDAP").Valid = False or DOC("NO_ADHERENT").Valid = False ) or adherentFounded = False ) then
	if( noAdhOk = false or anneeOk = false ) then
		'Correction du 06/06/2012 : Vu que dans PAPS nous n'utilisons pas les Corbeille, nous n'utiliserons pas cette valeur.
		'Doc("STATUS") = "CORBEILLE" ' On précise que le document va en corbeille
		Doc("STATUS") = "TORECORD"
		Doc("STATUS_INDEXATION") = "INDEXE" ' On précise que le document est bien indexé
	end if
	reportIndexDocRD("STATUS") ' On report à l'ensemble du document status (=&gt; en corbeille
	reportIndexDocRD("STATUS_INDEXATION") ' On report à l'ensemble du document le statut d'indexation (=&gt; INDEXE)

end function


'=========================================================================================================
' 3.3 - Fonctions de Report d'index - appelées par la Validation des champs
'=========================================================================================================

' Méthode qui reporte l'index sur l'ensemble de la transaction
Function ReportIndexTrans(Champ)
    call TraceDebug("fn","ReportIndexTrans","") 
	Dim i 'as long
	Dim Valeur ' a string
	Dim TheDoc ' as object
	
	Valeur = ""
	
	for i = 1 to trans.count

		Set TheDoc = TRANS(i)

		If i &lt;&gt; doc.Position and  TheDoc("TypeDocument") &lt;&gt; "ABC" then
			call TraceDebug("fn","data","F.Name=" &amp; F.Name) 

			Select case F.Name 
				case "OPERATEUR"
					Valeur = DOC("OPERATEUR")
					TheDoc("OPERATEUR") = Valeur
					TheDoc("OPERATEUR").Valid = True
				case "GESTIONNAIRE_REDAC"
					Valeur = DOC("GESTIONNAIRE_REDAC")
					TheDoc("GESTIONNAIRE_REDAC") = Valeur
					TheDoc("GESTIONNAIRE_REDAC").Valid = True
				case "CODE_GESTIONNAIRE_REDAC"
					Valeur = DOC("CODE_GESTIONNAIRE_REDAC")
					TheDoc("CODE_GESTIONNAIRE_REDAC") = Valeur
					TheDoc("CODE_GESTIONNAIRE_REDAC").Valid = True
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
				case "GROUPE"
					' GROUPE
					Valeur = DOC("GROUPE")
					TheDoc("GROUPE") = Valeur
					TheDoc("GROUPE").Valid = True
				case "NO_ADHERENT"
					' NO_ADHERENT
					Valeur = DOC("NO_ADHERENT")
					TheDoc("NO_ADHERENT") = Valeur
					TheDoc("NO_ADHERENT").Valid = True
				case "NO_DOSSIER"
					' NO_DOSSIER
					Valeur = DOC("NO_DOSSIER")
					TheDoc("NO_DOSSIER") = Valeur
					TheDoc("NO_DOSSIER").Valid = True
				case "CODE_DEPARTEMENT"
					' CODE_DEPARTEMENT
					Valeur = DOC("CODE_DEPARTEMENT")
					TheDoc("CODE_DEPARTEMENT") = Valeur
					TheDoc("CODE_DEPARTEMENT").Valid = True
				
		' Ajout JD : Traités -------------------------------
				
				case "DATE_NUM"
					Valeur = DOC("DATE_NUM")
					TheDoc("DATE_NUM") = Valeur
					TheDoc("DATE_NUM").Valid = True
				
				case "FAMILLE"
					Valeur = DOC("FAMILLE")
					TheDoc("FAMILLE") = Valeur
					TheDoc("FAMILLE").Valid = True
					
				case "MODE"
					Valeur = DOC("MODE")
					TheDoc("MODE") = Valeur
					TheDoc("MODE").Valid = True
				
				' ====================== Demande d'évolutions sur CPDAP : Report de l'index TYPE_DOC
				' TYPE_DOC
			'	case "TYPE_DOC"
			'		Valeur = DOC("TYPE_DOC")
			'		TheDoc("TYPE_DOC") = Valeur
			'		TheDoc("TYPE_DOC").Valid = True
				' COTE
				case "COTE"
					Valeur = DOC("COTE")
					TheDoc("COTE") = Valeur
					TheDoc("COTE").Valid = True
				' CONSERV_ORIGINAL
				case "CONSERV_ORIGINAL"
					Valeur = DOC("CONSERV_ORIGINAL")
					TheDoc("CONSERV_ORIGINAL") = Valeur
					TheDoc("CONSERV_ORIGINAL").Valid = True
				' VALEUR_JURIDIQUE
				case "VALEUR_JURIDIQUE"
					Valeur = DOC("VALEUR_JURIDIQUE")
					TheDoc("VALEUR_JURIDIQUE") = Valeur
					TheDoc("VALEUR_JURIDIQUE").Valid = True
				' DATE_DOCUMENT
				case "DATE_DOCUMENT"
					Valeur = DOC("DATE_DOCUMENT")
					TheDoc("DATE_DOCUMENT") = Valeur
					TheDoc("DATE_DOCUMENT").Valid = True
				' TYPE_ADHERENT
				case "TYPE_ADHERENT"
					Valeur = DOC("TYPE_ADHERENT")
					TheDoc("TYPE_ADHERENT") = Valeur
					TheDoc("TYPE_ADHERENT").Valid = True
				' PRESENCE_AR
				case "PRESENCE_AR"
					Valeur = DOC("PRESENCE_AR")
					TheDoc("PRESENCE_AR") = Valeur
					TheDoc("PRESENCE_AR").Valid = True
			End select
			
		End if
	next
	
End function

Function ReportForcedIndexTrans(Champ)
	call TraceDebug("fn","ReportForcedIndexTrans","") 
	Dim i 'as long
	Dim Valeur ' a string
	Dim StatutF ' a string	
	Dim StatutV ' a string		
	Dim TheDoc ' as object
	
	Valeur = ""
	
	for i = 1 to trans.count
		Set TheDoc = TRANS(i)

		If i &lt;&gt; doc.Position and  TheDoc("TypeDocument") &lt;&gt; "ABC" then
			Select case F.Name 
				case "GESTIONNAIRE_REDAC"
					' GESTIONNAIRE_REDAC
					Valeur  = DOC("GESTIONNAIRE_REDAC")
					StatutF = DOC("GESTIONNAIRE_REDAC").Forced
					StatutV = DOC("GESTIONNAIRE_REDAC").Valid
					TheDoc("GESTIONNAIRE_REDAC") = Valeur
					TheDoc("GESTIONNAIRE_REDAC").Valid = StatutV
					TheDoc("GESTIONNAIRE_REDAC").Forced = StatutF
				case "CODE_GESTIONNAIRE_REDAC"
					' CODE_GESTIONNAIRE_REDAC
					Valeur  = DOC("CODE_GESTIONNAIRE_REDAC")
					StatutF = DOC("CODE_GESTIONNAIRE_REDAC").Forced
					StatutV = DOC("CODE_GESTIONNAIRE_REDAC").Valid
					TheDoc("CODE_GESTIONNAIRE_REDAC") = Valeur
					TheDoc("CODE_GESTIONNAIRE_REDAC").Valid = StatutV
					TheDoc("CODE_GESTIONNAIRE_REDAC").Forced = StatutF
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
				case "GROUPE"
					' GROUPE
					Valeur  = DOC("GROUPE")
					StatutF = DOC("GROUPE").Forced
					StatutV = DOC("GROUPE").Valid
					TheDoc("GROUPE") = Valeur
					TheDoc("GROUPE").Valid = StatutV
					TheDoc("GROUPE").Forced = StatutF

				case "DATE_DOCUMENT"
					' DATE_DOCUMENT
					Valeur  = DOC("DATE_DOCUMENT")
					StatutF = DOC("DATE_DOCUMENT").Forced
					StatutV = DOC("DATE_DOCUMENT").Valid
					TheDoc("DATE_DOCUMENT") = Valeur
					TheDoc("DATE_DOCUMENT").Valid = StatutV
					TheDoc("DATE_DOCUMENT").Forced = StatutF
				case "NO_DOSSIER"
					' NO_DOSSIER
					Valeur  = DOC("NO_DOSSIER")
					StatutF = DOC("NO_DOSSIER").Forced
					StatutV = DOC("NO_DOSSIER").Valid
					TheDoc("NO_DOSSIER") = Valeur
					TheDoc("NO_DOSSIER").Valid = StatutV
					TheDoc("NO_DOSSIER").Forced = StatutF
				case "CODE_DEPARTEMENT"
					' CODE_DEPARTEMENT
					Valeur  = DOC("CODE_DEPARTEMENT")
					StatutF = DOC("CODE_DEPARTEMENT").Forced
					StatutV = DOC("CODE_DEPARTEMENT").Valid
					TheDoc("CODE_DEPARTEMENT") = Valeur
					TheDoc("CODE_DEPARTEMENT").Valid = StatutV
					TheDoc("CODE_DEPARTEMENT").Forced = StatutF
				case "NO_ADHERENT"
					' NO_ADHERENT
					Valeur  = DOC("NO_ADHERENT")
					StatutF = DOC("NO_ADHERENT").Forced
					StatutV = DOC("NO_ADHERENT").Valid
					TheDoc("NO_ADHERENT") = Valeur
					TheDoc("NO_ADHERENT").Valid = StatutV
					TheDoc("NO_ADHERENT").Forced = StatutF
			End select

			Select case F.Name 
				case "GESTIONNAIRE_REDAC"
					' GESTIONNAIRE_REDAC
					TheDoc("GESTIONNAIRE_REDAC").Forced = True
					TheDoc("GESTIONNAIRE_REDAC").Valid = False
				case "CODE_GESTIONNAIRE_REDAC"
					' CODE_GESTIONNAIRE_REDAC
					TheDoc("CODE_GESTIONNAIRE_REDAC").Forced = True
					TheDoc("CODE_GESTIONNAIRE_REDAC").Valid = False
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
					' PARTAGEABILITE
					TheDoc("PARTAGEABILITE").Forced = True
					TheDoc("PARTAGEABILITE").Valid = False
				case "GROUPE"
					' GROUPE
					TheDoc("GROUPE").Forced = True
					TheDoc("GROUPE").Valid = False
				case "DATE_DOCUMENT"
					' DATE_DOCUMENT
					TheDoc("DATE_DOCUMENT").Forced = True
					TheDoc("DATE_DOCUMENT").Valid = False
				'------------------- Modif du 11/08/2011 --------------
				case "NO_DOSSIER"
					' NO_DOSSIER
					TheDoc("NO_DOSSIER").Forced = True
					TheDoc("NO_DOSSIER").Valid = False
				case "CODE_DEPARTEMENT"
					' CODE_DEPARTEMENT
					TheDoc("CODE_DEPARTEMENT").Forced = True
					TheDoc("CODE_DEPARTEMENT").Valid = False
				case "NO_ADHERENT"
					' NO_ADHERENT
					TheDoc("NO_ADHERENT").Forced = True
					TheDoc("NO_ADHERENT").Valid = False
			End select

			
		End if
	next
	
End function


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

'=========================================================================================================
' 4 - Fonctions liées à ControlApresLecture - Appelée après LAD
'=========================================================================================================

Function ControlApresLecture()
	call TraceDebug("fn","ControlApresLecture","") 

	Dim FormulaireWex, PosTiret, TailleCodePiece, valLoc
	Dim debRefArchive
	Dim varDOSSIER
	Dim posSlash
	
	' ReferencePaquet pour la constitution de dossier
	call ConstitutionDocDTPP() ' 2.4.7
	
	'*** RECUPERATION DU CODE ORIGINE DU FORMULAIRE
	FormulaireWex = DOC("FormNameRecto")

	PosTiret = Instr(FormulaireWex, "_") 
	TailleCodePiece = Len(FormulaireWex) - PosTiret

	if ( len(FormulaireWex) &gt; PosTiret ) then
		Doc("UserField010") = Mid(FormulaireWex, PosTiret+1, TailleCodePiece)   ' Code Pièce		
	else
		Doc("UserField010") = ""
	end if

	call TraceDebug("data","ControlApresLecture01",""&amp;Doc("UserField010")) 

	'*** TRAITEMENT DU VERSO INUTILE : t_suppverso=0 VERSO NON EXPORTE TABLE CpTypeDoc
	if RecupTypeDoc(FormulaireWex) then
		Doc("ImageVerso") = G_suppverso   '0 Pour Ne Pas Exporter l'image Verso
	end if
	
	'*** NOM DU LOT : CCCCCPAAQQQLLL
	' Remplacement dans la REF_ARCHIVE DE 'MAFPJ'
	'Doc("CodeCondition") = replace(Doc("CodeCondition"),"MAFPJ","CPDAP")
	'Doc("UserField001") = Doc("CodeCondition")
	
	' On récupére le début de la référence Archive.
	'debRefArchive = Doc("CodeClient")
	debRefArchive = Doc("CodeBanque")
	
	' Remplacement dans la REF_ARCHIVE DE 'MAFPJ' par la partie spécifique au scénario (venant de PowerScan)
	Doc("CodeCondition") = replace(Doc("CodeCondition"),"MAFPJ",debRefArchive &amp; "_")
	Doc("UserField001") = Doc("CodeCondition")
		
	call RangDemandeDansLot() 
	'call GenerationNumeroDocRD() : retrait en 2.4.7
	
	varDOSSIER = Replace(Doc("lad_NUMDOSSIER")," ","")
	varDOSSIER = Replace(varDOSSIER,"'","")
	
	if len(varDOSSIER)&gt;0 then
		posSlash = InStr(varDOSSIER,"/")
		if posSlash&gt;0 then
			Doc("NO_DOSSIER") = replace(replace(left(varDOSSIER, posSlash)," ",""),"/","")
		else
			Doc("NO_DOSSIER") = ""
		end if
	end if
	
	Doc("FAMILLE") 				= "DOCUMENTS PAPS"
	Doc("TYPE_DOC") 			= "AR POSTE"
	Doc("LIB_DOC")				= "AR POSTE LA POSTE"
	Doc("DATE_NUM")				= Doc("DateTraitement")
	Doc("CODE_ORIGINE")			= "LAPOS"

	Doc("DATE_VISU")			= RIGHT(Doc("DateTraitement"),4)&amp; RIGHT(LEFT(Doc("DateTraitement"),5),2)&amp; LEFT(Doc("DateTraitement"),2)
	Doc("CODE_REDAC_VISU")		= Doc("CodeClient")
	Doc("DATE_QUALITE")			= Doc("DATE_VISU")
	Doc("VALIDE_QUALITE")		= "1"
	Doc("CODE_REDAC_QUALITE")	= Doc("CodeClient")

	
	Doc("STATUS") = "TORECORD" ' PAr défaut on envoi en GED
	
	Doc("MODE") = Doc("ReferenceBordereau") ' Transmis par Power Scan
	Doc("SERVICE") = Doc("ReferenceSession") ' Transmis par Power Scan
	
	if(Doc("MODE") = "COURRIER") then
		Doc("PRESENCE_AR") = 0
	elseif (Doc("MODE") = "COURRIER AR") then
		Doc("PRESENCE_AR") = 1
	end if
	
	Doc("STATUS_INDEXATION") = "INDEXE"
	
	Doc("VISIBILITE_EXTERNE") = 0 
	
	libOrigine = ""
	
	

	
End function


' Nouvelle fonction (03/2012) qui s'exécute avant le vidéo codage
function ControlAvantSaisie()
	call TraceDebug("fn","ControlAvantSaisie","") 
	
	' On renseigne le champ provenance avec une donnée en table S_OPEPROV
	if(getSpecialProvenance()) then
		Doc("PROVENANCE").Valid = true
	end if
end function








' --------------------------------------- Fonctions de Liste ---------------------------------------



' Liste NO_DOSSIER
Function ListNO_DOSSIERPlugin()
	call TraceDebug("fn","ListNO_DOSSIERPlugin","") 
	Dim plugin
	Dim nbenreg
	Dim SQL
	Dim ArrayRes ' tableau des champs resultats
	Dim RechercheBase
	
	Dim msg
	Dim retmsg
	Dim titre
	Dim NO_DOSSIER
	Dim NoTmp
	G_msg=""
	NO_DOSSIER = Doc("NO_DOSSIER")
	call TraceDebug("fn","ListNO_DOSSIERPlugin AVANT : ",NO_DOSSIER) 
	NO_DOSSIER = replace(NO_DOSSIER, " ", "")
	call TraceDebug("fn","ListNO_DOSSIERPlugin APRES : ",NO_DOSSIER) 
	
	' Bye si 0 car
	if (len(NO_DOSSIER) &lt; 2) then
		G_msg = "RAJOUTER PLUS DE CARACTERES NO_DOSSIER"
		ListNO_DOSSIERPlugin = false
		exit function
	end if
	
	' Si le Numéro de dossier fait au moins 4 caractéres et qu'il n'y a pas de lettre à gauche
	if( len(NO_DOSSIER) &gt;= 4 and isOnlyDigit(left(NO_DOSSIER, 1)) ) then
		SQL = SQL &amp; "Numero_Sinistre like '" &amp; NO_DOSSIER &amp; "%' " 
	else
		' Requetes SQL
		if (len(NO_DOSSIER) &gt;= 2) then
			SQL = "Code_societe = '" &amp; Mid(NO_DOSSIER, 1, 2) &amp; "' " 
		end if
		if (len(NO_DOSSIER) &gt;= 4) then
			SQL = SQL &amp; " and "
			SQL = SQL &amp; "Annee_survenance = '" &amp; Mid(NO_DOSSIER, 3, 2) &amp; "' " 
		end if
		if (len(NO_DOSSIER) &gt;= 7) then
			SQL = SQL &amp; " and "
			SQL = SQL &amp; "Code_Population = '" &amp; Mid(NO_DOSSIER, 5, 3) &amp; "' " 
		end if
		if (len(NO_DOSSIER) &gt;= 8) then
			SQL = SQL &amp; " and "
			'SQL = SQL &amp; "Numero_Sinistre like '" &amp; Mid(NO_DOSSIER, 8, 6) &amp; "%' "
			NoTmp = Mid(NO_DOSSIER, 8, 6) ' 8.2.6
			if not isOnlyDigit(right(NoTmp,1)) then NoTmp=left(NoTmp, len(NoTmp)-1) ' 8.2.6 : retrait de la clé sur c'est une lettre
'			SQL = SQL &amp; "Numero_Sinistre like '%" &amp; NoTmp &amp; "%' " ' 8.2.6 : sans la clé       'PAPSV12 mis en commentaire
			SQL = SQL &amp; "Numero_Sinistre like '" &amp; NoTmp &amp; "%' " ' 8.2.6 : sans la clé       'PAPSV12 suppression du caractere generique % en debut de chaine sur le like
			
		end if
	end if
	
	
   
	call TraceDebug("data","ListNO_DOSSIERPlugin",SQL) 
   
   ' Nombre d'enregistrements	
	If DB.RecordCreate("CountNbEnreg","Select count(*) as nbenreg from V_AFFAIRE_PAPS where " &amp; SQL) then
		nbenreg = DB.RecordValue("CountNbEnreg","nbenreg")
	end if																		
	call TraceDebug("data","ListNO_DOSSIERPlugin","nbenreg=" &amp; nbenreg &amp; ", MaxTop=" &amp; MaxTop) 
	if clng(nbenreg) &gt; clng(MaxTop) then
		G_msg = "RAJOUTER PLUS DE CARACTERES NO_DOSSIER (NB: " &amp; nbenreg &amp; ")"
		ListNO_DOSSIERPlugin = false
		exit function
	end if
	
	' Affiche la liste (Plugin)
	ListNO_DOSSIERPlugin = false
	    
	Set plugin =  AtomLib.InitPlugin("AtomVCDPlugin.SelectList")
	SQL = "Select Code_societe , Annee_survenance, Code_Population, Numero_Sinistre, Cle_sinistre, Service, Groupe, UPPER(Gestionnaire_redacteur), numero_Adherent, Code_Redacteur, Code_Departement from V_AFFAIRE_PAPS where " &amp; SQL &amp; " "
	SQL = SQL &amp; "order by Code_societe, Annee_survenance, Code_Population, Numero_Sinistre"
	nbenreg = clng(plugin.Load(SQL))
	titre = "LISTE DES DOSSIERS (TOTAL : " &amp; nbenreg &amp; ")"
	if nbenreg &gt; 1 then
		if ( G("Phase") &lt;&gt; "LAD" ) then
			If plugin.Afficher(titre,4000,0) then
				ListNO_DOSSIERPlugin = true
			end if
		end if
	elseif nbenreg = 1 then
			ListNO_DOSSIERPlugin = true
	else
		if ( G("Phase") &lt;&gt; "LAD" ) then
			G_msg =  "CE DOSSIER EST ABSENT DE LA BASE"
			ListNO_DOSSIERPlugin = false
		end if	
	end if

	' Recup enreg
	if (ListNO_DOSSIERPlugin = true) then
	
		ArrayRes = Split(plugin.resultat,"#",-1,1)
		Doc("NO_DOSSIER") = ArrayRes(0) &amp; ArrayRes(1) &amp; ArrayRes(2) &amp; ArrayRes(3) &amp; ArrayRes(4)
		
		'Evolution du 02/05/2012, on récupére le code du département
		Doc("CODE_DEPARTEMENT") = ArrayRes(10)
		
		'Seulement si le service n'est pas forcé
		if(Doc("SERVICE").forced = false) then 
			Doc("SERVICE") = ArrayRes(5)
		end if
		
		if(Doc("GROUPE").forced = false) then
			Doc("GROUPE") = ArrayRes(6)
		end if
		
		if(Doc("GESTIONNAIRE_REDAC").forced = false) then
			Doc("GESTIONNAIRE_REDAC") = ArrayRes(7)
		end if
		
		Doc("NO_ADHERENT") = ArrayRes(8) ' Lui on le renseigne dans tous les cas
		
		if(Doc("CODE_GESTIONNAIRE_REDAC").forced = false) then
			Doc("CODE_GESTIONNAIRE_REDAC") = ArrayRes(9)
		end if
		
		'PAPSV11 On affect le code gestionnaire du gestionnairte de numerisation si le type doc est bordereau de reglement
		if Doc("TYPE_DOC")="BORDEREAU DE REGLEMENT" then
			Doc("CODE_GESTIONNAIRE_REDAC") = Doc("CodeClient")
			Doc("GESTIONNAIRE_REDAC").valid = true
			Doc("CODE_GESTIONNAIRE_REDAC").valid  = true
			ReportIndexDocRD("CODE_GESTIONNAIRE_REDAC")
			ReportIndexDocRD("GESTIONNAIRE_REDAC")
		end if

		if Doc("TYPE_DOC")="NOTE HONORAIRES" then
			RechercheCodeRedacNoteHonoraire()
		end if
		
	end if
	
End Function

' Liste NO_DOSSIER lorsque le champ est forcé
Function ListNO_DOSSIERPluginForced()
	call TraceDebug("fn","ListNO_DOSSIERPluginForced","") 
	Dim plugin
	Dim nbenreg
	Dim SQL
	Dim ArrayRes ' tableau des champs resultats
	Dim RechercheBase
	
	Dim msg
	Dim retmsg
	Dim titre
	Dim NO_DOSSIER
	Dim NoTmp
	G_msg=""
	NO_DOSSIER = Doc("NO_DOSSIER")
	'call TraceDebug("fn","ListNO_DOSSIERPluginForced AVANT : ",NO_DOSSIER) 
	NO_DOSSIER = replace(NO_DOSSIER, " ", "")
	'call TraceDebug("fn","ListNO_DOSSIERPluginForced APRES : ",NO_DOSSIER) 
		
	' Bye si 0 car
	if (len(NO_DOSSIER) &lt; 2) then
		G_msg = "RAJOUTER PLUS DE CARACTERES NO_DOSSIER ou alors saisissez 0"
		ListNO_DOSSIERPluginForced = false
		exit function
	else
		if(isOnlyDigit(NO_DOSSIER) ) then
			SQL = " Numero_Sinistre LIKE '%" &amp; NO_DOSSIER &amp; "%'"
		else
			G_msg = "Champ forcé, ne mettez que le N° de dossier ou 0"
			ListNO_DOSSIERPluginForced = false
			exit function
		end if
		
	end if
	
   
	call TraceDebug("data","ListNO_DOSSIERPluginForced",SQL) 
   
   ' Nombre d'enregistrements	
	If DB.RecordCreate("CountNbEnreg","Select count(*) as nbenreg from V_AFFAIRE_PAPS where " &amp; SQL) then
		nbenreg = DB.RecordValue("CountNbEnreg","nbenreg")
	end if																		
	call TraceDebug("data","ListNO_DOSSIERPluginForced","nbenreg=" &amp; nbenreg &amp; ", MaxTop=" &amp; MaxTop) 
	if clng(nbenreg) &gt; clng(MaxTop) then
		G_msg = "RAJOUTER PLUS DE CARACTERES NO_DOSSIER (NB: " &amp; nbenreg &amp; ")"
		ListNO_DOSSIERPluginForced = false
		exit function
	end if
	
	' Affiche la liste (Plugin)
	ListNO_DOSSIERPluginForced = false
	    
	Set plugin =  AtomLib.InitPlugin("AtomVCDPlugin.SelectList")
	SQL = "Select Code_societe , Annee_survenance, Code_Population, Numero_Sinistre, Cle_sinistre, Service, Groupe, UPPER(Gestionnaire_redacteur), numero_Adherent, Code_Redacteur from V_AFFAIRE_PAPS where " &amp; SQL &amp; " "
	SQL = SQL &amp; "order by Code_societe, Annee_survenance, Code_Population, Numero_Sinistre"
	nbenreg = clng(plugin.Load(SQL))
	titre = "LISTE DES DOSSIERS (TOTAL : " &amp; nbenreg &amp; ")"
	if nbenreg &gt; 1 then
		if ( G("Phase") &lt;&gt; "LAD" ) then
			If plugin.Afficher(titre,4000,0) then
				ListNO_DOSSIERPluginForced = true
			end if
		end if
	elseif nbenreg = 1 then
			ListNO_DOSSIERPluginForced = true
	else
		if ( G("Phase") &lt;&gt; "LAD" ) then
			G_msg =  "CE DOSSIER EST ABSENT DE LA BASE"
			ListNO_DOSSIERPluginForced = false
		end if	
	end if

	' Recup enreg
	if (ListNO_DOSSIERPluginForced = true) then
		ArrayRes = Split(plugin.resultat,"#",-1,1)
		Doc("NO_DOSSIER") = ArrayRes(0) &amp; ArrayRes(1) &amp; ArrayRes(2) &amp; ArrayRes(3) &amp; ArrayRes(4)
		Doc("SERVICE") = ArrayRes(5)
		Doc("GROUPE") = ArrayRes(6)
		Doc("GESTIONNAIRE_REDAC") = ArrayRes(7)
		Doc("NO_ADHERENT") = ArrayRes(8)
		Doc("CODE_GESTIONNAIRE_REDAC") = ArrayRes(9)
	end if

	'PAPSV11 On affect le code gestionnaire du gestionnairte de numerisation si le type doc est bordereau de reglement
	if Doc("TYPE_DOC")="BORDEREAU DE REGLEMENT" then
		Doc("CODE_GESTIONNAIRE_REDAC") = Doc("CodeClient")
	end if

	if Doc("TYPE_DOC")="NOTE HONORAIRES" then
		RechercheCodeRedacNoteHonoraire()
	end if
	
End Function



' Liste GESTIONNAIRE_REDAC
Function ListGESTIONNAIRE_REDACForced_Plugin()
	call TraceDebug("fn","ListGESTIONNAIRE_REDACForced_Plugin","") 
	Dim plugin
	Dim nbenreg
	Dim SQL
	Dim ArrayRes ' tableau des champs resultats
	Dim RechercheBase
	
	Dim msg
	Dim retmsg
	Dim titre
	Dim CODEGEST_REDAC
	Dim NoTmp
	G_msg=""
	
	CODEGEST_REDAC = Doc("CODE_GESTIONNAIRE_REDAC")


	CODEGEST_REDAC = replace(CODEGEST_REDAC, " ", "")
	
	' Bye si 0 car
	if (len(CODEGEST_REDAC) &lt; 2) then
		G_msg = "RAJOUTER PLUS DE CARACTERES au code Redacteur"
		ListGESTIONNAIRE_REDACForced_Plugin = false
		exit function
	end if
	
   ' Requetes SQL
	if (len(CODEGEST_REDAC) &gt;= 2) then
		'SQL = " Code_Redacteur LIKE '%"&amp; CODEGEST_REDAC &amp;"%' OR Gestionnaire_Redacteur LIKE '% " &amp; GEST_REDAC &amp; " %' " 
		SQL = " Code_Redacteur = '"&amp; CODEGEST_REDAC &amp;"' " 
	end if
	
	' "select DISTINCT(GESTIONNAIRE_REDACTEUR) from V_GESTREDAC_SINI order by GESTIONNAIRE_REDACTEUR"
   
   ' Nombre d'enregistrements	
	If DB.RecordCreate("CountNbEnreg","Select count(*) as nbenreg from V_GESTREDAC_SINI where " &amp; SQL) then
		nbenreg = DB.RecordValue("CountNbEnreg","nbenreg")
	end if																		
	call TraceDebug("data","ListGESTIONNAIRE_REDACForced_Plugin","nbenreg=" &amp; nbenreg &amp; ", MaxTop=" &amp; MaxTop) 
	if clng(nbenreg) &gt; clng(MaxTop) then
		G_msg = "RAJOUTER PLUS DE CARACTERES GESTIONNAIRE_REDACT (NB: " &amp; nbenreg &amp; ")"
		ListGESTIONNAIRE_REDACForced_Plugin = false
		exit function
	end if
	
	' Affiche la liste (Plugin)
	ListGESTIONNAIRE_REDACForced_Plugin = false
	    
	Set plugin =  AtomLib.InitPlugin("AtomVCDPlugin.SelectList")
	SQL = "Select Code_Redacteur, Gestionnaire_Redacteur, Service, Groupe, Code_Redacteur_NoteHonoraire FROM V_GESTREDAC_SINI where " &amp; SQL &amp; " order by Code_Redacteur ASC"
	
	nbenreg = clng(plugin.Load(SQL))
	titre = "LISTE DES Gestionnaires / rédacteurs (TOTAL : " &amp; nbenreg &amp; ")"
	if nbenreg &gt; 1 then
		if ( G("Phase") &lt;&gt; "LAD" ) then
			If plugin.Afficher(titre,4000,0) then
				ListGESTIONNAIRE_REDACForced_Plugin = true
			end if
		end if
	elseif nbenreg = 1 then
			ListGESTIONNAIRE_REDACForced_Plugin = true
	else
		if ( G("Phase") &lt;&gt; "LAD" ) then
			G_msg =  "Ce Gestionnaire EST ABSENT DE LA BASE"
			ListGESTIONNAIRE_REDACForced_Plugin = false
		end if	
	end if

	' Recup enreg
	if (ListGESTIONNAIRE_REDACForced_Plugin = true) then
	
		ArrayRes = Split(plugin.resultat,"#",-1,1)
		Doc("CODE_GESTIONNAIRE_REDAC") = ArrayRes(0)
		Doc("GESTIONNAIRE_REDAC") = ArrayRes(1)
		Doc("SERVICE") = ArrayRes(2)
		Doc("GROUPE") = ArrayRes(3)
		
		'PAPSV11 On affect le code gestionnaire du gestionnairte de numerisation si le type doc est bordereau de reglement
		if Doc("TYPE_DOC")="BORDEREAU DE REGLEMENT" then
			Doc("CODE_GESTIONNAIRE_REDAC") = Doc("CodeClient")
		end if

		
		'29/05/2013 P9/C9
		if Doc("TYPE_DOC") = "NOTE HONORAIRES" then
			Doc("CODE_GESTIONNAIRE_REDAC") = ArrayRes(4)
			if len(Doc("CODE_GESTIONNAIRE_REDAC"))=0 then
				Doc("CODE_GESTIONNAIRE_REDAC") = "GHW"
			end if
		end if
		
	end if
	
End Function



' Méthode qui permet, de déduire du compte windows de l'utilisateur, 
function getSpecialProvenance()
	call TraceDebug("fn","getSpecialProvenance","") 
	
	' On récupére le compte NT de l'utilisateur
	dim compteNt 
	compteNt = UserNameWindows()
	
	Doc("PROVENANCE") = compteNt
	
	dim req
	dim resultsetName
	dim nbEnreg
	dim provPaps
	req = "SELECT count(PROVPAPS) as nbEnreg FROM V_SOPEPROV WHERE LOGIN_NT = '" &amp; compteNt &amp; "'"
	resultsetName = "CountNbEnreg"
	
	If DB.RecordCreate(resultsetName,req) then
		nbEnreg = DB.RecordValue(resultsetName,"nbEnreg")
	end if
	
	' Si l'utilisateur est dans la table
	if nbEnreg = 1 then
		req = "SELECT PROVPAPS FROM V_SOPEPROV WHERE LOGIN_NT = '" &amp; compteNt &amp; "'"
		resultsetName = "getProv"
		If DB.RecordCreate(resultsetName,req) then
			provPaps = DB.RecordValue(resultsetName,"PROVPAPS")
			Doc("PROVENANCE") = provPaps
			getSpecialProvenance = true
		end if
	else
		Doc("PROVENANCE") = ""
		getSpecialProvenance = false
		G_msg =  "OPERATEUR '" &amp; compteNt &amp; "' non présent dans 'V_SOPEPROV'"
	end if

end function





'---------------------- JD : function getVueAllAdherent -----------------------------
' Description : Cette fonction récupére la provenance du client depuis le numéro d'adhérent
' mais aussi le gestionnaire rédacteur associé, et le GROUPE et Service du gestionnaire
' rédacteur
'--------------------------------------------------------------------------------
function getVueAllAdherent(noAdh)

	call TraceDebug("fn","getVueAllAdherent",noAdh) 
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
			'DOC("PROVENANCE") = DB.RecordValue(Recordset,"FULL_NAME")
			DOC("PROVENANCE") =replace(DB.RecordValue(Recordset,"FULL_NAME"),"'"," ")
			DOC("GESTIONNAIRE_REDAC") = DB.RecordValue(Recordset,"LIB_GEST")
			DOC("GROUPE") = DB.RecordValue(Recordset,"GROUPE")
			DOC("SERVICE") = DB.RecordValue(Recordset,"SERVICE")
			getVueAllAdherent = true
		end if
	elseif NombreRedac = 0 then
		'G_msg = "ADHERENT INTROUVABLE EN BDD"
		MsgErreur = "ADHERENT INTROUVABLE EN BDD"
		DOC("PROVENANCE") = ""
		getVueAllAdherent = false
	else	
		G_msg = "V_ALL_ADHERENT : Doublons de NO_ADHERENT"
		MsgErreur = "V_ALL_ADHERENT : Doublons de NO_ADHERENT"
		getVueAllAdherent = false
	end if

end function


function getInfoRedac()
	call TraceDebug("fn","getInfoRedac","") 
	dim gRed
	gRed = Doc("GESTIONNAIRE_REDAC")
	G_msg = ""
	
	if(gRed &lt;&gt; "") then
		' Récup infos
		If DB.RecordCreate("VALRED","Select Code_Redacteur, Service, Groupe from V_GESTREDAC_SINI where Gestionnaire_Redacteur = '" &amp; gRed &amp; "'") then
			DOC("GROUPE") = DB.RecordValue("VALRED","Groupe")
			DOC("SERVICE") = DB.RecordValue("VALRED","Service")
			DOC("CODE_GESTIONNAIRE_REDAC") = DB.RecordValue("VALRED","Code_Redacteur")
			getInfoRedac = true
		end if
	else
		getInfoRedac = False
	end if

end function


function getInfoRedacByCode()
	call TraceDebug("fn","getInfoRedacByCode","") 
	dim gCod
	gCod = Doc("CODE_GESTIONNAIRE_REDAC")
	G_msg = ""
	
	if(gCod &lt;&gt; "") then
		' Récup infos
		If DB.RecordCreate("VALRED","Select Gestionnaire_Redacteur , Service, Groupe from V_GESTREDAC_SINI where Code_Redacteur = '" &amp; gCod &amp; "'") then
			DOC("GROUPE") = DB.RecordValue("VALRED","Groupe")
			DOC("SERVICE") = DB.RecordValue("VALRED","Service")
			DOC("GESTIONNAIRE_REDAC") = DB.RecordValue("VALRED","Gestionnaire_Redacteur")
			getInfoRedacByCode = true
		end if
	else
		getInfoRedacByCode = False
	end if

end function


' Modification du 02/12
' Cette fonction permet d'attribuer le gestionnaire/rédacteur + serv + gp annoncé dans PARAM_COTE
' Celle ci est appelée quand on attribue un type doc + origine
'function verifyWaitList()
'	call TraceDebug("fn","verifyWaitList","") 
'
'
'	dim famille
'	dim libTypeDoc
'	dim codeOri
'	dim nameResultSet
'	
'	' On récupére le type de document et le code origine
'	famille = Doc("FAMILLE") ' La famille est toujours 'DOCUMENTS PAPS'
'	libTypeDoc = Doc("TYPE_DOC")
'	codeOri = Doc("CODE_ORIGINE")
'	
'	
'	
'	
'	
'	
'	
'end function




Function RecupTypeDoc(FormulaireWex)
	call TraceDebug("fn","RecupTypeDoc","") 
	Dim NbEnreg
	
	NbEnreg = 0
	G_suppverso = 1
	G_priorite = 0
	RecupTypeDoc = false
		
	If DB.RecordCreate("CountTypeDoc","Select count(*) as NbEnreg from CpTypeDoc where t_codori= '" &amp; FormulaireWex &amp; "'") then
		NbEnreg = DB.RecordValue("CountTypeDoc","NbEnreg")
	end if
	
	if NbEnreg = 1 then
		If DB.RecordCreate("TypeDoc","Select t_type,t_priorite,t_suppverso from CpTypeDoc where t_codori= '" &amp; FormulaireWex &amp; "'") then

			Doc("UserField009") = DB.RecordValue("TypeDoc","t_type")
			G_suppverso = DB.RecordValue("TypeDoc","t_suppverso")
			G_priorite = DB.RecordValue("TypeDoc","t_priorite")				
			RecupTypeDoc = true
		end if
	end if
		
End function

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
	
	NbEnreg = 0
	Rang = 0
	NbTotal = 0
	
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

Function ConstitutionDocDTPP()
	' 2.4.7
	Dim posdoc
	Dim TheDoc ' as object
	Dim SuivDoc ' as object

	ConstitutionDocDTPP = False
	
	Doc("ReferencePaquet") = Doc("NumeroDocEntree")
	posdoc = Doc.Position
	
	' Champ ReferencePaquet permet de constituer les documents (SEPDOC)(PIECE)*

	if (posdoc &gt; 1) and  (Doc("TypeDocument") = "PAPS1") then
		Set TheDoc = TRANS(posdoc-1)
		
		Select case TheDoc("TypeDocument") 
			case "PAPS1"
				Doc("ReferencePaquet") = TheDoc("ReferencePaquet")				
			case else
			
		End select		
		
	else
		if (posdoc &gt; 1) and (Doc("TypeDocument") = "SEPDOC") then
			Set TheDoc = TRANS(posdoc-1)
			
			Select case TheDoc("TypeDocument") 
					
				case "PAPS1"		
				case else
				
			End select		
				
		else
		end if
	end if

	'MsgBox " POS= " &amp; posdoc &amp; " ReferencePaquet= " &amp; Doc("ReferencePaquet") &amp; " Numero= " &amp; TheDoc("NumeroDocEntree")
	
	ConstitutionDocDTPP = True
	
End function


'=========================================================================================================
' 5 - Fonctions Générales - Appelées par Atome : Paramétrées dans le Déclaratif 
'=========================================================================================================


' Fonction qui affiche le cadre d'information en bas, dans Menu5.exe
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

Function PositionDocRD()
	call TraceDebug("fn","PositionDocRD","") 

	Dim i 'as long
	Dim Pos 'as long
	Dim Resultat 'as long

	Pos = 1
	
	' msgbox Doc.Position,16,"POSITION DOC" 
	
	' on va du doc courant vers le 1er doc jusqu'a rupture
	for i = Doc.Position - 1  to 1 step - 1
		' if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then exit for
		if TRANS.Value(i,"ReferencePaquet") &lt;&gt; Doc("ReferencePaquet") then exit for ' 2.4.7
		Pos = Pos + 1
	next

	Resultat = cstr(Pos)
	
	for i = Doc.Position + 1  to TRANS.Count
		' if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then exit for
		if TRANS.Value(i,"ReferencePaquet") &lt;&gt; Doc("ReferencePaquet") then exit for ' 2.4.7
		Pos = Pos + 1
	next

	Resultat = Resultat &amp; " / " &amp; cstr(Pos)
	
	PositionDocRD = cstr(Resultat)
	
End function

' CRTL F5
Function DupliqueChamp()

 Dim retour

	retour = DupliqueChampPrec(F.Name)
	
	Doc(F.Name) = retour
	
	'DupliqueChamp = retour

End Function

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


Function TraceDebugSAV(TypeTrace, fonction, Chaine)
	'msgbox(fonction &amp; " : " &amp; Chaine)
End Function

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
	strTmp = DateJour &amp; ";Phase=" &amp; Phase &amp; ";DocEncours=" &amp; DocumentEnCours &amp; ";" &amp; TypeTrace &amp; ";" &amp; fonction &amp; ": " &amp; Chaine &amp; vbCrLf
	
   ' Ouverture
	Set fso = CreateObject("Scripting.FileSystemObject")
	Set f = fso.OpenTextFile("C:\POST1.log", ForAppend, true)
   
	' Ecriture conditionnelle
	Select case TypeTrace
		' passage dans les fonctions
		case "fn"
			f.write(strTmp)
		case "DEBUG"
			'f.write(strTmp)
		' Ecriture des données en cours
		case "data"
'			f.write(strTmp)
	End select
	
	on error goto 0
	
End Function

' Check a string if they are only DIGIT's
'   Return TRUE for only DIGIT
' v.2.6.6
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
Function DocExit()

	DocExit = True

	if ( G("Phase") &lt;&gt; "LAD" ) then
		' Compte le nbre de page du Document
		CountPAGE()
		ReportIndexDocRD("PAGE")
		Doc("OPERATEUR")=UserNameWindows() ' 2.5.2
		ReportIndexDocRD("OPERATEUR") ' 2.5.2
		
		' 22/04/2015
		if ( G("Phase") = "VCD" ) then
			Doc("OPERATEUR_INDEXATION")=RechercheTrigrammeSurCompteNT(Doc("OPERATEUR"))
			ReportIndexTrans("OPERATEUR_INDEXATION") 
		end if
	end if

End Function

Function CountPAGE()
	' 2.5.0 : on tient compte des doc annulés
	Dim i 'as long
	Dim Valeur ' a string
	Dim TheDoc ' as object
	Dim nbpage 
	
	Valeur = ""
	nbpage = 1
	
	' on va du doc courant vers le 1er doc jusqu'a rupture
	for i = Doc.Position - 1  to 1 step - 1
		if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then exit for
		if TRANS.Value(i,"ImageRecto")="True" then
		nbpage = nbpage + 1
		end if
		if TRANS.Value(i,"ImageVerso")="True" then
		nbpage = nbpage + 1
		end if
	next

	' on va du doc courant vers le dernier doc jusqu'a rupture
	for i = Doc.Position + 1  to TRANS.Count
		if TRANS.Value(i,"FormNameRecto") &lt;&gt; Doc("FormNameRecto") then exit for
		if TRANS.Value(i,"ImageRecto")="True" then
		nbpage = nbpage + 1
		end if 
		if TRANS.Value(i,"ImageVerso")="True" then
		nbpage = nbpage + 1 
		end if
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

' Fonction alertInVcd
' Détails: Cette fonction permet d'afficher un "Alert" durant la phase de video-codage.
' Note : Si un MsgBox est demandé durant les phase de RAD &amp; LAD, la phase ne peut aboutir.
' C'est pourquoi on controle que l'on est  bien en phase VCD
function alertInVcd(mesg)
	if( G("Phase") = "VCD") then
		MsgBox(mesg)
	end if
	Exit function
end function

'Fonction : isValidField
'Détails : Cette fonction return true si un champ est validé ou false si celui ci est invalide ou forcé
function isValidField(anyDocField)
	call TraceDebug("fn","isValidField","") 

	' Si le champ est valide &amp;amp; pas forcé, on retourne true
	'if(anyDocField.Valid and anyDocField.Forced = false) then 
	if(anyDocField.Valid) then 
		isValidField = True
	' Sinon on retourne false
	else 
		isValidField = False
	end if

end function


' Méthode getTheCote()
' Permet de déduire une cote, depuis un type_doc + un code_origine
function getTheCote()
	dim td 
	dim codeOr
	dim sqlReq
	dim nbRes
	
	td = Doc("TYPE_DOC")
	codeOr = Doc("CODE_ORIGINE")
	G_msg =  ""
	

	' Si on a un type de document et un code origine
	if(td &lt;&gt; "" and codeOr &lt;&gt; "") then
		sqlReq = "SELECT Count(vrc.LIB_COTE) as nbResults"
		sqlReq = sqlReq &amp; " FROM V_REF_COTE vrc"
		sqlReq = sqlReq &amp; " inner join V_PARAM_COTE vpc on vrc.CODE_COTE = vpc.CODE_COTE"
		sqlReq = sqlReq &amp; " inner join C_CHARTEDOC cc on cc.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC"
		sqlReq = sqlReq &amp; " WHERE cc.TYPE_DOC = '"&amp; td &amp;"' AND vpc.CODE_ORIGINE = '"&amp; codeOr &amp;"' AND cc.ACTIVITE LIKE '%PAPS%'"
		
		' On compte le nombre de cotes correspondant
		If DB.RecordCreate("CountCodeCote",sqlReq) then
			nbRes = DB.RecordValue("CountCodeCote","nbResults")
		end if
		
		if(nbRes = 1) then 
			sqlReq = "SELECT vrc.LIB_COTE"
			sqlReq = sqlReq &amp; " FROM V_REF_COTE vrc"
			sqlReq = sqlReq &amp; " inner join V_PARAM_COTE vpc on vrc.CODE_COTE = vpc.CODE_COTE"
			sqlReq = sqlReq &amp; " inner join C_CHARTEDOC cc on cc.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC"
			sqlReq = sqlReq &amp; " WHERE cc.TYPE_DOC = '"&amp; td &amp;"' AND vpc.CODE_ORIGINE = '"&amp; codeOr &amp;"' AND ACTIVITE LIKE '%PAPS%'"
		
			If DB.RecordCreate("getCodeCote",sqlReq) then
				DOC("COTE") = DB.RecordValue("getCodeCote","LIB_COTE")
				getTheCote = true
				MsgErreur = ""
			else
				MsgErreur = "Erreur de la requete SQL getTheCote()"
				getTheCote = false
			end if
		elseif nbRes &gt; 1 then
			MsgErreur =  "Erreur, doublons dans le paramétrage(1) des cotes getTheCote() "
			getTheCote = false
		else
			MsgErreur =  "Pas de cote pour ce type de doc et cette origine"
			getTheCote = false
		end if
	
	else
		MsgErreur =  "Saisir un type de doc et une origine ou forcer"
		getTheCote = false
	end if

end function

' Fonction qui permet de stocker dans une variable globale le libellé du code origine choisi
function getTheLibOrigine()
	call TraceDebug("fn","getTheLibOrigine","") 

	dim sqlReq
	dim codeOri

	
	codeOri = Doc("CODE_ORIGINE")
	
	if(len(codeOri) &gt; 0 ) then

		sqlReq = "SELECT LIB_ORIGINE FROM V_REF_ORIGINE WHERE CODE_ORIGINE = '" &amp; codeOri &amp; "'"
		
		If DB.RecordCreate("getLibOri",sqlReq) then
			libOrigine = DB.RecordValue("getLibOri","LIB_ORIGINE")


			getTheLibOrigine = true
			MsgErreur = ""
		else
			MsgErreur = "Erreur de la requete SQL getTheLibOrigine()"

			getTheCote = false
		end if

	else
		MsgErreur = "Le code origine n'est pas renseigné"
		getTheLibOrigine = false
	end if


end function



'Fonction : checkStatut
'Détails : Cette fonction applique le statut( En ged ou en corbeille), en fonction du renseignement ou non des autres champs
function checkStatus()
	call TraceDebug("fn","checkStatus","") 
	
	'if( (len(Doc("NO_CONTRAT")) &gt; 0)  and isValidField(Doc("NO_ADHERENT")) and isValidField(Doc("DATE_DOCUMENT")) ) then 
'	if( isValidField(Doc("NO_CONTRAT")) and isValidField(Doc("NO_ADHERENT")) and isValidField(Doc("DATE_DOCUMENT")) and isValidField(Doc("NO_AVENANTLAD")) ) then 
'		DOC("STATUS") = "TORECORD"
'		Doc("STATUS_INDEXATION") = "INDEXE"
'		G_msg = "" ' On vide le message
'		checkStatus = true
'	else
'		DOC("STATUS") = "CORBEILLE"
'		Doc("STATUS_INDEXATION") = "NON INDEXE"
'		G_msg = "Un des champs n'est pas conforme, le document ira en corbeille"
'		checkStatus = false
'	end if

	if(isValidField(Doc("FAMILLE")) and isValidField(Doc("TYPE_DOC")) and isValidField(Doc("NO_DOSSIER")) and isValidField(Doc("NO_ADHERENT")) and isValidField(Doc("GESTIONNAIRE_REDAC")) and isValidField(Doc("CODE_ORIGINE"))) then
		DOC("STATUS") = "TORECORD"
		Doc("STATUS_INDEXATION") = "INDEXE"
		G_msg = "" ' On vide le message
		checkStatus = true
	end if



end function



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

Function CleModulo(Source, themodulo )
    Dim complement ' As Long

	complement = themodulo - Modulo(Source, themodulo )

	CleModulo = complement
	
End Function


Function RequeteListeNatureDoc()

	Dim CodePiece      
	Dim SQL
	
	CodePiece = Doc("UserField010")
	
	SQL = "select e_code, e_libelle, e_codepiece from CpamNatureDoc where e_codepiece = '" &amp; CodePiece &amp; "' order by e_code "

	If DB.RecordCreate("Count",SQL) = false then
		CodePiece = "DEFAUT"
		RequeteListeNatureDoc = "select e_code, e_libelle, e_codepiece from CpamNatureDoc where e_codepiece = '" &amp; CodePiece &amp; "' order by e_code "
	else
		RequeteListeNatureDoc = "select e_code, e_libelle, e_codepiece from CpamNatureDoc where e_codepiece = '" &amp; CodePiece &amp; "' order by e_code "
	end if

End function

Function RechercheCodeRedacNoteHonoraire()
	Call TraceDebug("fn","RechercheCodeRedacNoteHonoraire","DEB")

	Dim CodeRedac
	Dim CodeRedacHonoraire
	Dim NbEnr
	Dim SQL

	RechercheCodeRedacNoteHonoraire = False

	SQL = "SELECT Count(*) AS NbEnreg FROM LADMAF.dbo.V_AFFAIRE_PAPS AS AFFAIRE, LADMAF.dbo.V_GESTREDAC_SINI AS GEST "
	SQL = SQL &amp; "WHERE AFFAIRE.Code_Redacteur=GEST.Code_Redacteur AND NO_DOSSIER = '" &amp; Doc("NO_DOSSIER") &amp; "'"
	
	If DB.RecordCreate("CountGestHonoraire", SQL) then
		NbEnr = DB.RecordValue("CountGestHonoraire","NbEnreg")
	end if
	
	if NbEnr&gt;0 then
	'	SQL = "SELECT Code_Redacteur_NoteHonoraire FROM V_GESTREDAC_SINI WHERE Code_Redacteur = '" &amp; CodeRedacActuel &amp; "'"
		SQL = "SELECT GEST.Code_Redacteur, GEST.Code_Redacteur_NoteHonoraire FROM LADMAF.dbo.V_AFFAIRE_PAPS AS AFFAIRE, LADMAF.dbo.V_GESTREDAC_SINI AS GEST "
		SQL = SQL &amp; "WHERE AFFAIRE.Code_Redacteur=GEST.Code_Redacteur AND NO_DOSSIER = '" &amp; Doc("NO_DOSSIER") &amp; "'"

		If DB.RecordCreate("RecupGestHonoraire", SQL) then
			CodeRedac 			= DB.RecordValue("RecupGestHonoraire","Code_Redacteur")
			CodeRedacHonoraire 	= DB.RecordValue("RecupGestHonoraire","Code_Redacteur_NoteHonoraire")
			
'			msgbox("CodeRedac : " &amp; CodeRedac &amp; " ---- CodeRedacHonoraire : " &amp; CodeRedacHonoraire)
				'PAPSV11 On affect le code gestionnaire du gestionnairte de numerisation si le type doc est bordereau de reglement
			if Doc("TYPE_DOC")="BORDEREAU DE REGLEMENT" then
				Doc("CODE_GESTIONNAIRE_REDAC") = Doc("CodeClient")
			end if
		
			if Doc("TYPE_DOC")="NOTE HONORAIRES" then
				If len(CodeRedacHonoraire)=0 then
					CodeRedacHonoraire = "GHW"
				end if
				Doc("CODE_GESTIONNAIRE_REDAC") = CodeRedacHonoraire
			else
				Doc("CODE_GESTIONNAIRE_REDAC") = CodeRedac
			end if
		end if
	else
		RechercheCodeRedacNoteHonoraire = False
		exit function
	end if

	'CODE_GESTIONNAIRE_REDAC_Validation()
	RechercheCodeRedacNoteHonoraire = True
	
End Function


' créée le 22/04/2015 WTH
' permet de mettre le code trigramme de l'operatur d'indexation dans la donnée DEPOSER PAR dans PAPS
Function RechercheTrigrammeSurCompteNT(CompteNT)
	Dim Trig
	Dim SQL
	Dim NbEnr
	
	SQL = "select count(Code_Redacteur) as NbEnreg From V_ALL_UTILISATEUR where compte_nt = '" &amp; CompteNT &amp; "'"

	If DB.RecordCreate("Count",SQL) then
		NbEnr = DB.RecordValue("Count","NbEnreg")
	end if

	if NbEnr=1 then
		SQL = "select Code_Redacteur From V_ALL_UTILISATEUR where compte_nt = '" &amp; CompteNT &amp; "'"
		if DB.RecordCreate("RecupTrigrame",SQL) then
			Trig = DB.RecordValue("RecupTrigrame","Code_Redacteur")
		else
			RechercheTrigrammeSurCompteNT = "ERREUR"
			exit function
		end if
	else
		RechercheTrigrammeSurCompteNT = "ERREUR"
		exit function
	end if	
	
	RechercheTrigrammeSurCompteNT = Trig
	
End Function